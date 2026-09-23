using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniB2B.Models;
using MiniB2B.Services;
using MiniB2B.ViewModels;

namespace MiniB2B.Controllers;

[Authorize]
public class ProductController : Controller
{
    private const string AdminRole = "Admin";

    private readonly ProductService _productService;
    private readonly ISliderService _sliderService;
    private readonly GridConfigService _gridConfigService;

    public ProductController(
        ProductService productService,
        ISliderService sliderService,
        GridConfigService gridConfigService)
    {
        _productService = productService;
        _sliderService = sliderService;
        _gridConfigService = gridConfigService;
    }

    public async Task<IActionResult> Index(string? aramaKelimesi, int? kategoriId)
    {
        var isAdmin = User.IsInRole(AdminRole);

        var products = await _productService.SearchAsync(aramaKelimesi, kategoriId);
        var columns = await _gridConfigService.GetVisibleColumnsAsync(isAdmin);
        var kategoriler = await _productService.GetCategoriesAsync();
        var featuredIds = await _sliderService.GetFeaturedProductIdsAsync();

        var vm = new ProductGridViewModel
        {
            Products = products,
            Columns = columns,
            AramaKelimesi = aramaKelimesi,
            KategoriId = kategoriId,
            Kategoriler = kategoriler,
            FeaturedIds = featuredIds,
            IsAdmin = isAdmin
        };

        return View(vm);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null) return NotFound();

        ViewBag.BenzerUrunler = await _productService.GetSimilarByBrandAsync(product);
        return View(product);
    }

    [HttpGet]
    [Authorize(Roles = AdminRole)]
    public async Task<IActionResult> Create()
    {
        await LoadCategoriesAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = AdminRole)]
    public async Task<IActionResult> Create(Product product, IFormFile? resimDosyasi)
    {
        if (!ModelState.IsValid)
        {
            await LoadCategoriesAsync();
            return View(product);
        }

        await _productService.CreateAsync(product, resimDosyasi);
        TempData["Mesaj"] = "Ürün başarıyla eklendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = AdminRole)]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null) return NotFound();

        await LoadCategoriesAsync();
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = AdminRole)]
    public async Task<IActionResult> Edit(Product product, IFormFile? resimDosyasi)
    {
        if (!ModelState.IsValid)
        {
            await LoadCategoriesAsync();
            return View(product);
        }

        if (!await _productService.UpdateAsync(product, resimDosyasi))
            return NotFound();

        TempData["Mesaj"] = "Ürün başarıyla güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadCategoriesAsync() =>
        ViewBag.Kategoriler = await _productService.GetCategoriesAsync();
}