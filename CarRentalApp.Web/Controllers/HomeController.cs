using CarRentalApp.Business.Services;
using CarRentalApp.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CarRentalApp.Web.Controllers;

// Bu sayfalara sadece giriş yapmış (Oturum açmış) kullanıcılar girebilir
[Authorize] 
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
    var userIdString = User.FindFirst("UserId")?.Value;
    if (int.TryParse(userIdString, out int userId))
    {
        ViewBag.CurrentUser = await _userService.GetUserByIdAsync(userId);
    }

    // 5 parametreyi birden gönderiyoruz. Hata burada çözülecek:
    var cars = await _carService.SearchCarsAsync(brand, model, minPrice, maxPrice, isAvailable);

    // Filtreleri ekranda geri göstermek için:
    ViewBag.Brand = brand;
    ViewBag.Model = model;
    ViewBag.MinPrice = minPrice;
    ViewBag.MaxPrice = maxPrice;
    ViewBag.IsAvailable = isAvailable;

    return View(cars);
}
}