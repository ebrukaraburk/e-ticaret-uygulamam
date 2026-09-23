using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MiniB2B.Models;
using MiniB2B.Services;

namespace MiniB2B.Controllers;

public class AccountController : Controller
{
    private readonly UserService _userService;

    public AccountController(UserService userService)
    {
        _userService = userService;
    }



    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string? usernameOrEmail, string? password, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (string.IsNullOrWhiteSpace(usernameOrEmail) || string.IsNullOrWhiteSpace(password))
        {
            ViewBag.Error = "Lütfen tüm alanları doldurunuz.";
            return View();
        }

        var user = await _userService.AuthenticateAsync(usernameOrEmail, password);
        if (user == null)
        {
            ViewBag.Error = "Kullanıcı adı/e-posta veya şifre hatalı!";
            return View();
        }

        await SignInAsync(user);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Product");
    }



    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // Bind sayesinde Role gibi alanlar formdan gönderilemez, herkes "Customer" olarak kaydolur
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(
        [Bind("FirstName,LastName,Username,Email,PhoneNumber")] User user, string? password)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Error = "Lütfen bilgilerinizi kontrol edin.";
            return View(user);
        }

        if (string.IsNullOrWhiteSpace(password) || password.Length < UserService.MinPasswordLength)
        {
            ViewBag.Error = $"Şifre en az {UserService.MinPasswordLength} karakter olmalıdır.";
            return View(user);
        }

        if (await _userService.ExistsAsync(user.Username, user.Email))
        {
            ViewBag.Error = "Bu kullanıcı adı veya e-posta adresi zaten kullanımda.";
            return View(user);
        }

        await _userService.CreateAsync(user, password);

        TempData["Mesaj"] = "Kayıt başarıyla oluşturuldu. Şimdi giriş yapabilirsiniz.";
        return RedirectToAction(nameof(Login));
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    public IActionResult AccessDenied()
    {
        return View();
    }

    // Kullanıcı bilgilerini ve rolünü cookie'ye yazar
    private async Task SignInAsync(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
    }
}