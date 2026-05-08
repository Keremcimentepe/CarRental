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
    public async Task<IActionResult> Create(User user, IFormFile profileImage)
    {
        // 1. ADIM: Ekranda resim yüklenmiş mi diye kontrol ediyoruz
        if (profileImage != null && profileImage.Length > 0)
        {
            // 2. ADIM: Resmi hafıza akışına (MemoryStream) alıyoruz
            using (var memoryStream = new MemoryStream())
            {
                await profileImage.CopyToAsync(memoryStream);
                // 3. ADIM: Hafızadaki resmi Byte (BLOB) dizisine çevirip User modeline atıyoruz
                user.ProfilePicture = memoryStream.ToArray();
            }
        }

        // Eğer rol seçilmemişse varsayılan olarak normal kullanıcı (User) yapıyoruz
        if (string.IsNullOrEmpty(user.Role))
        {
            user.Role = "User";
        }

        // Veritabanına kaydet (Business katmanı üzerinden)
        await _userService.AddUserAsync(user);

        // Kayıt başarılıysa Login ekranına yönlendir
        return RedirectToAction("Login", "Auth");
    }
}