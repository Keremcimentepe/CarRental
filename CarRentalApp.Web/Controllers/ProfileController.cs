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

    public async Task<IActionResult> Index()
{
    var userIdString = User.FindFirst("UserId")?.Value;
    if (string.IsNullOrEmpty(userIdString)) return RedirectToAction("Login", "Auth");

    var userId = int.Parse(userIdString);
    
    // Servisi çağırıyoruz
    var allRentals = await _rentalService.GetAllRentalsAsync();

    // Gider: Benim kiraladıklarım
    var myExpenses = allRentals.Where(r => r.UserId == userId).ToList();

    // Gelir: Benim arabalarımı kiralayanlar
    var myIncomes = allRentals.Where(r => r.Car != null && r.Car.OwnerId == userId).ToList();

    ViewBag.TotalIncome = myIncomes.Sum(r => r.TotalPrice);
    ViewBag.TotalExpense = myExpenses.Sum(r => r.TotalPrice);
    ViewBag.Incomes = myIncomes;
    ViewBag.Expenses = myExpenses;

    return View();
}
}