using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniB2B.Models;
using MiniB2B.Services;

namespace MiniB2B.Controllers;

[Authorize]
public class UserController : Controller
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index()
    {
        var users = await _userService.GetAllAsync();
        return View(users);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Details(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null) return NotFound();

        ViewBag.SavedCard = await _userService.GetSavedCardAsync(id);
        return View(user);
    }

    // Kullanıcının KENDİ profili — herhangi bir giriş yapmış kullanıcı erişebilir
    public async Task<IActionResult> Profile()
    {
        int userId = GetCurrentUserId();

        var user = await _userService.GetByIdAsync(userId);
        if (user == null) return NotFound();

        ViewBag.SavedCard = await _userService.GetSavedCardAsync(userId);
        return View("Details", user);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null) return NotFound();

        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(User user, string? newPassword)
    {
        if (!ModelState.IsValid)
            return View(user);

        if (await _userService.ExistsAsync(user.Username, user.Email, user.Id))
        {
            ModelState.AddModelError(nameof(user.Username), "Bu kullanıcı adı veya e-posta başka bir kullanıcıya ait.");
            return View(user);
        }

        if (!await _userService.UpdateAsync(user, newPassword))
            return NotFound();

        TempData["Mesaj"] = "Kullanıcı bilgileri başarıyla güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    private int GetCurrentUserId()
    {
        var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claimValue, out int userId) ? userId : 1;
    }
}