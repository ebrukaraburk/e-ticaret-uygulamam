using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniB2B.Data;
using MiniB2B.Models;
using MiniB2B.Services;
using System.Security.Claims;

namespace MiniB2B.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly OrderService _orderService;
        private readonly UserService _userService;

        public OrderController(AppDbContext context, OrderService orderService, UserService userService)
        {
            _context = context;
            _orderService = orderService;
            _userService = userService;
        }

        // 1. Sepetim Sayfası
        public IActionResult CartIndex() => View(GetBasket());

        // 2. Sepete Ekleme
        [HttpPost]
        public IActionResult AddToCart(int productId, int adet)
        {
            TempData["Mesaj"] = _orderService.SepeteUrunEkle(GetCurrentUserId(), productId, adet);
            return RedirectToAction("Details", "Product", new { id = productId });
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int itemId)
        {
            TempData["Mesaj"] = _orderService.SepettenUrunCikar(GetCurrentUserId(), itemId);
            return RedirectToAction("CartIndex");
        }

        [HttpPost]
        public IActionResult UpdateCartQuantity(int itemId, int delta)
        {
            TempData["Mesaj"] = _orderService.SepetUrunAdetGuncelle(GetCurrentUserId(), itemId, delta);
            return RedirectToAction("CartIndex");
        }

        // Kullanıcının kendi siparişini iptal etmesi
        [HttpPost]
        public IActionResult CancelOrder(int id)
        {
            TempData["Mesaj"] = _orderService.SiparisIptalEt(GetCurrentUserId(), id);
            return RedirectToAction("MyOrders");
        }

        [HttpPost]
        public IActionResult ConfirmOrder() => RedirectToAction("Payment");

        // Ödeme sayfasını gösterir
        [HttpGet]
        public IActionResult Payment()
        {
            var basket = GetBasket();
            if (basket == null || !basket.Items.Any())
            {
                TempData["Mesaj"] = "Sepetiniz boş.";
                return RedirectToAction("CartIndex");
            }

            return View(basket);
        }

        // Kart bilgilerini doğrular, adres + maskelenmiş kart bilgisini kaydeder, siparişi oluşturur
        [HttpPost]
        public async Task<IActionResult> Payment(
            string cardHolder, string cardNumber, string expiry, string cvv,
            string addressLine, string city, string postalCode)
        {
            var basket = GetBasket();
            if (basket == null || !basket.Items.Any())
            {
                TempData["Mesaj"] = "Sepetiniz boş.";
                return RedirectToAction("CartIndex");
            }

            string? hata = KartDogrula(cardHolder, cardNumber, expiry, cvv);
            if (hata != null)
            {
                ViewBag.Hata = hata;
                return View(basket);
            }

            if (string.IsNullOrWhiteSpace(addressLine) || string.IsNullOrWhiteSpace(city))
            {
                ViewBag.Hata = "Teslimat adresi zorunludur.";
                return View(basket);
            }

            int userId = GetCurrentUserId();

            // Adresi ve maskelenmiş kart bilgisini profile kaydet
            await _userService.SaveAddressAsync(userId, addressLine, city, postalCode);

            var digits = new string(cardNumber.Where(char.IsDigit).ToArray());
            var last4 = digits.Length >= 4 ? digits[^4..] : digits;
            var expiryParts = expiry.Split('/');
            await _userService.SaveCardAsync(userId, cardHolder, last4, expiryParts[0], expiryParts[1]);

            string sonuc = _orderService.SiparisOlusturFromBasket(userId);
            if (!sonuc.StartsWith("Siparişiniz başarıyla"))
            {
                TempData["Mesaj"] = sonuc;
                return RedirectToAction("CartIndex");
            }

            TempData["Mesaj"] = "Ödemeniz alındı. " + sonuc;
            return RedirectToAction("MyOrders");
        }

        // 4. Siparişlerim
        public IActionResult MyOrders()
        {
            var orders = _context.Orders
                .Where(o => o.UserId == GetCurrentUserId())
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
        }

        // 5. Sipariş detay sayfası
        public IActionResult Details(int id)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.User)
                .AsQueryable();

            if (!User.IsInRole("Admin"))
                query = query.Where(o => o.UserId == GetCurrentUserId());

            var order = query.FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                TempData["Mesaj"] = "Sipariş bulunamadı.";
                return RedirectToAction("MyOrders");
            }

            return View(order);
        }

        // Sipariş durumunu güncelleme (yönetici) - ilerlet veya reddet
        [HttpPost]
        public IActionResult UpdateOrderStatus(int id, string status)
        {
            TempData["Mesaj"] = status switch
            {
                "İlerlet" => _orderService.SiparisSonrakiAsamayaGecir(id),
                var s when s == MiniB2B.Models.OrderStatus.Rejected => _orderService.SiparisReddet(id),
                _ => "Geçersiz işlem."
            };

            return RedirectToAction("Orders");
        }

        // Yönetici sipariş listesi (Views/Admin/Orders.cshtml dosyasını gösterir)
        public IActionResult Orders()
        {
            var orders = _context.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View("~/Views/Admin/Orders.cshtml", orders);
        }

        // --- Yardımcılar ---

        private Basket? GetBasket() =>
            _context.Baskets
                .Include(b => b.Items).ThenInclude(i => i.Product)
                .FirstOrDefault(b => b.UserId == GetCurrentUserId());

        // Giriş yapan kullanıcının ID'sini dinamik olarak okur
        private int GetCurrentUserId()
        {
            var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claimValue, out int userId) ? userId : 1;
        }

        private static string? KartDogrula(string? holder, string? number, string? expiry, string? cvv)
        {
            if (string.IsNullOrWhiteSpace(holder))
                return "Kart üzerindeki isim boş olamaz.";

            string digits = new string((number ?? "").Where(char.IsDigit).ToArray());
            if (digits.Length < 13 || digits.Length > 19 || !LuhnGecerli(digits))
                return "Kart numarası geçersiz.";

            var parts = (expiry ?? "").Split('/');
            if (parts.Length != 2
                || !int.TryParse(parts[0], out int ay)
                || !int.TryParse(parts[1], out int yil)
                || ay < 1 || ay > 12)
                return "Son kullanma tarihi geçersiz (AA/YY olmalı).";

            if (yil < 100) yil += 2000;
            if (yil > 2100) return "Son kullanma tarihi geçersiz.";

            var sonGun = new DateTime(yil, ay, 1).AddMonths(1).AddDays(-1);
            if (sonGun < DateTime.Today)
                return "Kartın son kullanma tarihi geçmiş.";

            if (cvv == null || cvv.Length < 3 || cvv.Length > 4 || !cvv.All(char.IsDigit))
                return "CVV geçersiz.";

            if (digits == "4000000000000002")
                return "Ödeme banka tarafından reddedildi. Lütfen başka bir kart deneyin.";

            return null;
        }

        private static bool LuhnGecerli(string digits)
        {
            int sum = 0;
            bool ikiyleCarp = false;
            for (int i = digits.Length - 1; i >= 0; i--)
            {
                int n = digits[i] - '0';
                if (ikiyleCarp)
                {
                    n *= 2;
                    if (n > 9) n -= 9;
                }
                sum += n;
                ikiyleCarp = !ikiyleCarp;
            }
            return sum % 10 == 0;
        }
    }
}