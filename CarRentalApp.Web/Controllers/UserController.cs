using CarRentalApp.Business.Services;
using CarRentalApp.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace CarRentalApp.Web.Controllers;

public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [HttpPost]
public async Task<IActionResult> Create(User user, IFormFile profileImage)
{
    // --- 1. KULLANICI ADI VE ŞİFRE UZUNLUK KONTROLÜ ---
    if (user.Username.Length < 3 || user.Username.Length > 20)
    {
        ViewBag.ErrorMessage = "Kullanıcı adı 3 ile 20 karakter arasında olmalıdır!";
        return View(user);
    }

    if (user.Password.Length < 6 || user.Password.Length > 20)
    {
        ViewBag.ErrorMessage = "Şifreniz güvenliğiniz için 6 ile 20 karakter arasında olmalıdır!";
        return View(user);
    }

    // --- 2. AYNI İSİMDE KULLANICI VAR MI KONTROLÜ ---
    var allUsers = await _userService.GetAllUsersAsync();
    bool isUsernameTaken = allUsers.Any(u => u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase));
    
    if (isUsernameTaken)
    {
        ViewBag.ErrorMessage = "Bu kullanıcı adı zaten kullanılıyor! Lütfen başka bir kullanıcı adı seçin.";
        return View(user);
    }

    // --- 3. RESİM YÜKLEME VE KAYDETME İŞLEMİ (Önceki Kodlar) ---
    if (profileImage != null && profileImage.Length > 0)
    {
        using (var memoryStream = new MemoryStream())
        {
            await profileImage.CopyToAsync(memoryStream);
            user.ProfilePicture = memoryStream.ToArray();
        }
    }

    if (string.IsNullOrEmpty(user.Role))
    {
        user.Role = "User";
    }

    await _userService.AddUserAsync(user);
    return RedirectToAction("Login", "Auth");
}
}