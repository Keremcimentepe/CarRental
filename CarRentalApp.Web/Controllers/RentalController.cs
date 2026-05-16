using CarRentalApp.Business.Services;
using CarRentalApp.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CarRentalApp.Web.Controllers;

[Authorize]
public class RentalController : Controller
{
    private readonly IRentalService _rentalService;
    private readonly ICarService _carService;

    public RentalController(IRentalService rentalService, ICarService carService)
    {
        _rentalService = rentalService;
        _carService = carService;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int carId)
    {
        if (User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value == "Admin") 
        return RedirectToAction("Index", "Home");
        var car = await _carService.GetCarByIdAsync(carId);
        if (car == null || !car.IsAvailable) return RedirectToAction("Index", "Home");

        ViewBag.Car = car;
        return View();
    }

    [HttpPost]
[HttpPost]
public async Task<IActionResult> Create(int carId, DateTime startDate, DateTime endDate)
{
    var car = await _carService.GetCarByIdAsync(carId);
    var userIdString = User.FindFirst("UserId")?.Value;
    
    if (string.IsNullOrEmpty(userIdString)) return RedirectToAction("Login", "Auth");
    var userId = int.Parse(userIdString);

    // PostgreSQL için UTC formatına çeviriyoruz
    startDate = DateTime.SpecifyKind(startDate.Date, DateTimeKind.Utc);
    endDate = DateTime.SpecifyKind(endDate.Date, DateTimeKind.Utc);
    DateTime todayUtc = DateTime.SpecifyKind(DateTime.Now.Date, DateTimeKind.Utc);

    // --- 1. KURAL: Başlangıç tarihi bugünden itibaren en fazla 3 gün sonra olabilir ---
    int daysUntilStart = (startDate - todayUtc).Days;
    if (daysUntilStart < 0 || daysUntilStart > 3)
    {
        ViewBag.Car = car;
        ViewBag.ErrorMessage = "Hata: Kiralama başlangıç tarihi bugünden itibaren en fazla 3 gün içerisinde olmalıdır!";
        return View();
    }

    // --- 2. KURAL: Bitiş tarihi, başlangıç tarihinden önce veya aynı olamaz ---
    if (endDate <= startDate)
    {
        ViewBag.Car = car;
        ViewBag.ErrorMessage = "Hata: Bitiş tarihi, başlangıç tarihinden en az 1 gün sonra olmalıdır!";
        return View();
    }

    // --- 3. KURAL: Maksimum 30 gün kiralama yapılabilir ---
    int days = (endDate - startDate).Days;
    if (days > 30)
    {
        ViewBag.Car = car;
        ViewBag.ErrorMessage = "Hata: Bir aracı tek seferde en fazla 30 gün kiralayabilirsiniz!";
        return View();
    }

    // --- v2.0 FİNANSAL KOMİSYON HESAPLAMASI ---
    decimal grossTotal = car.DailyPrice * days; // Toplam tutar (Brüt)

    var rental = new Rental
    {
        CarId = carId,
        UserId = userId,
        StartDate = startDate,
        EndDate = endDate,
        TotalPrice = grossTotal,
        CommissionAmount = grossTotal * 0.10m, // %10 Şirket Payı
        SellerAmount = grossTotal * 0.90m      // %90 Satıcı Kazancı
    };

    await _rentalService.RentCarAsync(rental);
    return RedirectToAction("Index", "Home");
}
}