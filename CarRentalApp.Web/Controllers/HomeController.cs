using CarRentalApp.Business.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CarRentalApp.Web.Controllers;

// [Authorize] YOKTUR. Misafirler de ana sayfayı görebilir.
public class HomeController : Controller
{
    private readonly IUserService _userService;
    private readonly ICarService _carService;

    public HomeController(IUserService userService, ICarService carService)
    {
        _userService = userService;
        _carService = carService;
    }
    public IActionResult About()
{
    return View();
}

    public async Task<IActionResult> Index(string brand, string model, decimal? minPrice, decimal? maxPrice, bool? isAvailable, int page = 1)
    {
        int pageSize = 6; // Bir sayfada gösterilecek araç sayısı

        // 1. Servisten hem araç listesini hem de toplam onaylı araç sayısını alıyoruz
        var (cars, totalCount) = await _carService.SearchCarsAsync(brand, model, minPrice, maxPrice, isAvailable, page, pageSize);

        // 2. Sayfalama matematiği ve ViewBag atamaları
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        ViewBag.Brand = brand;
        ViewBag.Model = model;
        ViewBag.MinPrice = minPrice;
        ViewBag.MaxPrice = maxPrice;
        ViewBag.IsAvailable = isAvailable;

        // 3. Giriş yapan kullanıcının bilgilerini çekme (Misafirse null kalır)
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
            ViewBag.CurrentUser = null; 
        }

        return View(cars);
    }
}