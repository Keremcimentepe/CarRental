using CarRentalApp.Business.Services;
using CarRentalApp.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CarRentalApp.Web.Controllers;

// Bu sayfalara sadece giriş yapmış (Oturum açmış) kullanıcılar girebilir
public class HomeController : Controller
{
    private readonly IUserService _userService;
    private readonly ICarService _carService;

    public HomeController(IUserService userService, ICarService carService)
    {
        _userService = userService;
        _carService = carService;
    }

    // Arama kelimesini (keyword) parametre olarak alıyoruz
    public async Task<IActionResult> Index(string brand, string model, decimal? minPrice, decimal? maxPrice, bool? isAvailable)
    {
        // 1. Sadece giriş yapmışsa kullanıcı bilgilerini çek
        if (User.Identity.IsAuthenticated)
        {
            var userIdString = User.FindFirst("UserId")?.Value;
            if (int.TryParse(userIdString, out int userId))
            {
                ViewBag.CurrentUser = await _userService.GetUserByIdAsync(userId);
            }
        }
        else
        {
            ViewBag.CurrentUser = null; // Misafir
        }

        // 2. Sadece Onaylı araçları getir (Service tarafında ayarladık)
        var cars = await _carService.SearchCarsAsync(brand, model, minPrice, maxPrice, isAvailable);

        ViewBag.Brand = brand; ViewBag.Model = model; ViewBag.MinPrice = minPrice;
        ViewBag.MaxPrice = maxPrice; ViewBag.IsAvailable = isAvailable;

        return View(cars);
    }
}