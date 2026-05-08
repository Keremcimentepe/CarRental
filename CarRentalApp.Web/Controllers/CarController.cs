using CarRentalApp.Business.Services;
using CarRentalApp.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace CarRentalApp.Web.Controllers;

[Authorize] // Bu sayfalara sadece giriş yapanlar erişebilir
public class CarController : Controller 
{
    private readonly ICarService _carService;

    public CarController(ICarService carService)
    {
        _carService = carService;
    }

    // --- YENİ ARAÇ EKLEME (CREATE) ---
[HttpPost]
public async Task<IActionResult> Create(Car car, IFormFile carImage)
{
    // 1. Giriş yapmış kullanıcının ID'sini Claims üzerinden alıyoruz
    var userIdClaim = User.FindFirst("UserId")?.Value;

    if (userIdClaim != null)
    {
        // Arabanın sahibi olarak login olan kişiyi atıyoruz
        car.OwnerId = int.Parse(userIdClaim);
    }
    else
    {
        // Eğer kullanıcı login değilse (ki [Authorize] ile korunmalı), hata verebiliriz
        return RedirectToAction("Login", "Auth");
    }

    // 2. Resim yükleme işlemi (Önceki kodların aynısı)
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
[HttpPost]
[ValidateAntiForgeryToken] // Güvenlik için ekleyelim
public async Task<IActionResult> Delete(int id)
{
    var car = await _carService.GetCarByIdAsync(id);
    if (car == null) return NotFound();

    await _carService.DeleteCarAsync(id);
    return RedirectToAction("Index", "Home");
}

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    

    // --- ARAÇ GÜNCELLEME (UPDATE) ---
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var car = await _carService.GetCarByIdAsync(id);
        if (car == null) return NotFound();
        
        return View(car);
    }

    

    // --- ARAÇ GÜNCELLEME (POST) ---
[HttpPost]
public async Task<IActionResult> Edit(Car car, IFormFile carImage)
{
    // 1. Veritabanındaki eski aracı bul (OwnerId ve Image'i kaybetmemek için)
    var existingCar = await _carService.GetCarByIdAsync(car.Id);
    if (existingCar == null) return NotFound();

    // 2. Kullanıcıdan gelen yeni bilgileri mevcut aracın üzerine yazıyoruz
    existingCar.Brand = car.Brand;
    existingCar.Model = car.Model;
    existingCar.Year = car.Year;
    existingCar.DailyPrice = car.DailyPrice;
    existingCar.IsAvailable = car.IsAvailable;

    // 3. Eğer yeni bir resim yüklendiyse onu da güncelle
    if (carImage != null && carImage.Length > 0)
    {
        using (var memoryStream = new MemoryStream())
        {
            await carImage.CopyToAsync(memoryStream);
            existingCar.Image = memoryStream.ToArray();
        }
    }

    // 4. Güncellenmiş existingCar nesnesini veritabanına kaydet
    await _carService.UpdateCarAsync(existingCar);
    return RedirectToAction("Index", "Home");
}
}