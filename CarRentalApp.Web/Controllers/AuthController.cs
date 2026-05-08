using CarRentalApp.Business.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CarRentalApp.Web.Controllers;

public class AuthController : Controller
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        // Business katmanındaki Login metodumuzu çağırıyoruz
        var user = await _userService.LoginAsync(username, password);

        if (user != null)
        {
            // Kullanıcı bulunduysa kimlik (Claim) bilgilerini oluşturuyoruz
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role ?? "User"),
                new Claim("UserId", user.Id.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Çerezi oluşturup kullanıcıyı sisteme giriş yaptırıyoruz
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }

        // Hatalı girişte kullanıcıya mesaj gösterilmesi (Proje isteri)
        ViewBag.ErrorMessage = "Kullanıcı adı veya şifre hatalı!";
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Auth");
    }
}