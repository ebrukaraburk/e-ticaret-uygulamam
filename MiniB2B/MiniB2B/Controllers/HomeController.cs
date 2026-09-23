using Microsoft.AspNetCore.Mvc;
using MiniB2B.Services;
using System.Threading.Tasks;

namespace MiniB2B.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISliderService _sliderService;
        private readonly ProductService _productService;

        public HomeController(ISliderService sliderService, ProductService productService)
        {
            _sliderService = sliderService;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var sliders = await _sliderService.GetActiveSlidersAsync();

            var tumUrunler = await _productService.SearchAsync(null, null);
            ViewBag.TumUrunler = tumUrunler;

            return View(sliders);
        }
    }
}