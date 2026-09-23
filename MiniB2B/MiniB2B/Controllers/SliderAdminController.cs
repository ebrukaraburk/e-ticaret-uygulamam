using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniB2B.Models;
using MiniB2B.Services;
using System.Threading.Tasks;

namespace MiniB2B.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SliderAdminController : Controller
    {
        private readonly ISliderService _sliderService;
        private readonly ProductService _productService;

        public SliderAdminController(ISliderService sliderService, ProductService productService)
        {
            _sliderService = sliderService;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var sliders = await _sliderService.GetAllSlidersAsync();
            return View(sliders);
        }

        // GET: Boş açılırsa afiş, productId ile gelirse ürün öne çıkarma
        [HttpGet]
        public async Task<IActionResult> Create(int? productId)
        {
            if (productId is > 0 && await _sliderService.IsFeaturedAsync(productId.Value))
                return AlreadyFeaturedRedirect();

            var model = new SliderItem { IsActive = true, DisplayOrder = 1 };

            if (productId is > 0)
            {
                var product = await _productService.GetByIdAsync(productId.Value);
                if (product != null)
                {
                    model.ProductId = product.Id;
                    model.Title = product.ProductName;
                    model.ImageUrl = product.ImageUrl;
                    model.LinkUrl = $"/Product/Details/{product.Id}";
                }
            }

            ViewBag.ProductId = productId;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderItem slider, IFormFile? imageFile, int? productId)
        {
            // Ürün öne çıkarma ile geldiyse ürünle ilişkilendir
            if (productId is > 0)
            {
                if (await _sliderService.IsFeaturedAsync(productId.Value))
                    return AlreadyFeaturedRedirect();

                slider.ProductId = productId.Value;

                // Yeni dosya seçilmediyse ürünün resmini kullan
                if (imageFile == null || imageFile.Length == 0)
                {
                    var product = await _productService.GetByIdAsync(productId.Value);
                    if (product != null)
                        slider.ImageUrl = product.ImageUrl;
                }
            }

            // Otomatik gelen zorunluluk hatalarını temizleyip kendi kontrolümüzü yapıyoruz
            ModelState.Remove("imageFile");
            ModelState.Remove("ImageUrl");
            ModelState.Remove("LinkUrl");

            if (string.IsNullOrEmpty(slider.ImageUrl) && (imageFile == null || imageFile.Length == 0))
                ModelState.AddModelError("ImageUrl", "Lütfen bir afiş görseli seçiniz.");

            if (!ModelState.IsValid)
            {
                ViewBag.ProductId = productId;
                return View(slider);
            }

            await _sliderService.AddSliderAsync(slider, imageFile);

            if (productId.HasValue)
            {
                TempData["Mesaj"] = "Ürün başarıyla öne çıkarıldı.";
                return RedirectToAction("Index", "Product");
            }

            TempData["Mesaj"] = "Afiş başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        // Ürünü öne çıkarılanlardan kaldır
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFeatured(int productId)
        {
            await _sliderService.RemoveByProductIdAsync(productId);
            TempData["Mesaj"] = "Ürün öne çıkarılanlardan kaldırıldı.";
            return RedirectToAction("Index", "Product");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _sliderService.DeleteSliderAsync(id);
            TempData["Mesaj"] = "Kayıt silindi.";
            return RedirectToAction(nameof(Index));
        }
        private IActionResult AlreadyFeaturedRedirect()
        {
            TempData["Mesaj"] = "Bu ürün zaten öne çıkarılmış.";
            return RedirectToAction("Index", "Product");
        }
    }
}