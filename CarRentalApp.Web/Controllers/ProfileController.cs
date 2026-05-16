using Microsoft.AspNetCore.Authorization;
using CarRentalApp.Business.Services;
using CarRentalApp.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CarRentalApp.Web.Controllers;
[Authorize]
public class ProfileController : Controller
{
    private readonly IRentalService _rentalService;
    private readonly ICarService _carService;
    private readonly IUserService _userService;

    public ProfileController(IRentalService rentalService, ICarService carService, IUserService userService)
    {
        _rentalService = rentalService;
        _carService = carService;
        _userService = userService;
    }
    [HttpGet]
public async Task<IActionResult> Edit()
{
    var userId = int.Parse(User.FindFirst("UserId").Value);
    var user = await _userService.GetUserByIdAsync(userId);
    return View(user);
}

[HttpPost]
public async Task<IActionResult> Edit(User updatedUser, IFormFile profileImage)
{
    var userId = int.Parse(User.FindFirst("UserId").Value);
    var user = await _userService.GetUserByIdAsync(userId);

    user.Username = updatedUser.Username; // İsim değişikliği (Unique kontrolü eklenebilir)
    
    if (profileImage != null)
    {
        using var ms = new MemoryStream();
        await profileImage.CopyToAsync(ms);
        user.ProfilePicture = ms.ToArray();
    }

    await _userService.UpdateUserAsync(user);
    return RedirectToAction("Index");
}

    public async Task<IActionResult> Index()
{
    var userIdString = User.FindFirst("UserId")?.Value;
    if (string.IsNullOrEmpty(userIdString)) return RedirectToAction("Login", "Auth");

    var userId = int.Parse(userIdString);
    var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

    var allRentals = await _rentalService.GetAllRentalsAsync();
    
    // View tarafında rolü bilmek için ViewBag'e atıyoruz
    ViewBag.Role = role;

    if (role == "Admin")
    {
        // --- ADMİN v2.0 ANALİTİĞİ ---
        ViewBag.TotalCompanyProfit = allRentals.Sum(r => r.CommissionAmount); // Şirketin net karı (%10'ların toplamı)
        ViewBag.TotalGrossVolume = allRentals.Sum(r => r.TotalPrice);        // Platformdaki toplam para dönüşü

        // Pie Chart için satıcı gelir istatistikleri (En çok kazandıranlar)
        var sellerStats = allRentals
            .Where(r => r.Car != null && r.Car.Owner != null)
            .GroupBy(r => r.Car.Owner.Username)
            .Select(g => new { Username = g.Key, TotalEarned = g.Sum(r => r.SellerAmount) })
            .OrderByDescending(x => x.TotalEarned)
            .ToList();

        ViewBag.SellerLabels = sellerStats.Select(s => s.Username).ToList();
        ViewBag.SellerData = sellerStats.Select(s => s.TotalEarned).ToList();
        ViewBag.AllTransactions = allRentals.OrderByDescending(r => r.StartDate).ToList();
    }
    else
    {
        // --- KİRALAYICI VE USER FİNANS TAKİBİ ---
        var myExpenses = allRentals.Where(r => r.UserId == userId).ToList();
        var myIncomes = allRentals.Where(r => r.Car != null && r.Car.OwnerId == userId).ToList();

        ViewBag.TotalIncome = myIncomes.Sum(r => r.SellerAmount); // Artık brüt değil, %90'lık net gelirini görüyor
        ViewBag.TotalExpense = myExpenses.Sum(r => r.TotalPrice); // Gider her zaman brüttür (tam para ödenir)
        ViewBag.Incomes = myIncomes;
        ViewBag.Expenses = myExpenses;
    }

    return View();
}
}