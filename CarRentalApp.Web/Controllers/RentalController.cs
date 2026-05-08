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
        var car = await _carService.GetCarByIdAsync(carId);
        if (car == null || !car.IsAvailable) return RedirectToAction("Index", "Home");

        ViewBag.Car = car;
        return View();
    }

    [HttpPost]
public async Task<IActionResult> Create(int carId, DateTime startDate, DateTime endDate)
{
    var car = await _carService.GetCarByIdAsync(carId);
    var userId = int.Parse(User.FindFirst("UserId").Value);

    // ÇÖZÜM: HTML'den gelen tarihleri PostgreSQL için UTC formatına çeviriyoruz
    startDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
    endDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);

    // Gün farkını hesapla ve toplam fiyatı belirle
    int days = (endDate - startDate).Days;
    if (days <= 0) days = 1;

    var rental = new Rental
    {
        CarId = carId,
        UserId = userId,
        StartDate = startDate,
        EndDate = endDate,
        TotalPrice = car.DailyPrice * days
    };

    await _rentalService.RentCarAsync(rental);
    return RedirectToAction("Index", "Home");
}
}