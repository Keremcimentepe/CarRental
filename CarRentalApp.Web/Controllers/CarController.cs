using CarRentalApp.Business.Services;
using CarRentalApp.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Security.Claims;

namespace CarRentalApp.Web.Controllers;

[Authorize] 
public class CarController : Controller 
{
    private readonly ICarService _carService;

    public CarController(ICarService carService)
    {
        _carService = carService;
    }

    // --- YENİ ARAÇ EKLEME (GET) ---
    [HttpGet]
    public IActionResult Create()
    {
        // Sadece Kiralayıcı araç ekleme sayfasını görebilir
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        if (role != "Kiralayıcı") return RedirectToAction("Index", "Home");
        
        return View();
    }

    // --- YENİ ARAÇ EKLEME (POST) ---
    [HttpPost]
    public async Task<IActionResult> Create(Car car, IFormFile carImage)
    {
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        if (role != "Kiralayıcı") return RedirectToAction("Index", "Home");

        var userIdClaim = User.FindFirst("UserId")?.Value;
        if (userIdClaim != null)
        {
            car.OwnerId = int.Parse(userIdClaim);
        }
        else
        {
            return RedirectToAction("Login", "Auth");
        }

        if (carImage != null && carImage.Length > 0)
        {
            using (var memoryStream = new MemoryStream())
            {
                await carImage.CopyToAsync(memoryStream);
                car.Image = memoryStream.ToArray();
            }
        }

        await _carService.AddCarAsync(car);
        return RedirectToAction("Index", "Home");
    }

    // --- ARAÇ GÜNCELLEME (GET) ---
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var car = await _carService.GetCarByIdAsync(id);
        if (car == null) return NotFound();

        var userId = int.Parse(User.FindFirst("UserId").Value);
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        // Sadece Admin VEYA aracın sahibi sayfayı görebilir
        if (role != "Admin" && car.OwnerId != userId) return RedirectToAction("Index", "Home");
        
        return View(car);
    }

    // --- ARAÇ GÜNCELLEME (POST) ---
    [HttpPost]
    public async Task<IActionResult> Edit(Car car, IFormFile carImage)
    {
        var existingCar = await _carService.GetCarByIdAsync(car.Id);
        if (existingCar == null) return NotFound();

        var userId = int.Parse(User.FindFirst("UserId").Value);
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        // Post edilirken de sadece Admin veya aracın sahibi değiştirebilir
        if (role != "Admin" && existingCar.OwnerId != userId) return RedirectToAction("Index", "Home");

        existingCar.Brand = car.Brand;
        existingCar.Model = car.Model;
        existingCar.Year = car.Year;
        existingCar.DailyPrice = car.DailyPrice;
        existingCar.IsAvailable = car.IsAvailable;

        if (carImage != null && carImage.Length > 0)
        {
            using (var memoryStream = new MemoryStream())
            {
                await carImage.CopyToAsync(memoryStream);
                existingCar.Image = memoryStream.ToArray();
            }
        }

        await _carService.UpdateCarAsync(existingCar);
        return RedirectToAction("Index", "Home");
    }

    // --- ARAÇ SİLME (POST) ---
    [HttpPost]
    [ValidateAntiForgeryToken] 
    public async Task<IActionResult> Delete(int id)
    {
        var car = await _carService.GetCarByIdAsync(id);
        if (car == null) return NotFound();

        var userId = int.Parse(User.FindFirst("UserId").Value);
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        // Sadece Admin VEYA aracın kendi sahibi silebilir
        if (role == "Admin" || car.OwnerId == userId)
        {
            await _carService.DeleteCarAsync(id);
        }
        
        return RedirectToAction("Index", "Home");
    }
}