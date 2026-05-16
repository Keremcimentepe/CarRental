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
    // 1. Kullanıcının girdiği şifreyi SHA-256 ile kriptoluyoruz
    string hashedPassword = "";
    using (System.Security.Cryptography.SHA256 sha256Hash = System.Security.Cryptography.SHA256.Create())
    {
        byte[] bytes = sha256Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        for (int i = 0; i < bytes.Length; i++)
        {
            builder.Append(bytes[i].ToString("x2"));
        }
        hashedPassword = builder.ToString();
    }

    // 2. Kriptolanmış şifre ile Business katmanındaki Login metodumuzu çağırıyoruz
    var user = await _userService.LoginAsync(username, hashedPassword);

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