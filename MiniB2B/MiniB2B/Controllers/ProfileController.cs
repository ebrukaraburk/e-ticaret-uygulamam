using Microsoft.AspNetCore.Mvc;
using MiniB2B.Models;
using MiniB2B.Services;
using System.Security.Claims;

namespace MiniB2B.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserService _userService;

        public ProfileController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userService.GetByIdAsync(GetCurrentUserId());
            if (user == null) return RedirectToAction("Login", "Account");

            var savedCard = await _userService.GetSavedCardAsync(GetCurrentUserId());

            var vm = new ProfileViewModel
            {
                User = user,
                SavedCard = savedCard
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAddress(string addressLine, string city, string? postalCode)
        {
            if (string.IsNullOrWhiteSpace(addressLine) || string.IsNullOrWhiteSpace(city))
            {
                TempData["Mesaj"] = "Adres ve şehir zorunludur.";
                return RedirectToAction("Index");
            }

            await _userService.SaveAddressAsync(GetCurrentUserId(), addressLine, city, postalCode);
            TempData["Mesaj"] = "Adres bilgileriniz güncellendi.";
            return RedirectToAction("Index");
        }

        private int GetCurrentUserId()
        {
            var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claimValue, out int userId) ? userId : 1;
        }
    }
}