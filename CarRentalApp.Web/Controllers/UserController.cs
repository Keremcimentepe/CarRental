using CarRentalApp.Business.Services;
using CarRentalApp.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

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
public async Task<IActionResult> Create(User user, IFormFile profileImage, string adminCode)
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
    // --- ŞİFRE KRİPTOLAMA (SHA-256) ---
using (SHA256 sha256Hash = SHA256.Create())
{
    byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(user.Password));
    StringBuilder builder = new StringBuilder();
    for (int i = 0; i < bytes.Length; i++)
    {
        builder.Append(bytes[i].ToString("x2"));
    }
    if (user.Role == "Admin" && adminCode != "admin")
    {
        ViewBag.ErrorMessage = "Hata: Admin hesabı oluşturmak için yetki kodunuz yanlış!";
        return View(user);
    }
    user.Password = builder.ToString(); // Artık veritabanına "123456" yerine karmaşık bir şifre gidecek
}
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