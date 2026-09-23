using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using MiniB2B.Data;
using MiniB2B.Models;

namespace MiniB2B.Services;

public class ProductService
{
    private const string ImageFolder = "minib2b_products";

    private readonly AppDbContext _context;
    private readonly Cloudinary _cloudinary;

    public ProductService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;

        _cloudinary = new Cloudinary(new Account(
            configuration["CloudinarySettings:CloudName"],
            configuration["CloudinarySettings:ApiKey"],
            configuration["CloudinarySettings:ApiSecret"]));
    }

    public async Task<List<Product>> SearchAsync(string? keyword, int? categoryId)
    {
        var query = _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            keyword = keyword.Trim().ToLower();

            query = query.Where(p =>
                p.ProductName.ToLower().Contains(keyword) ||
                p.ProductCode.ToLower().Contains(keyword) ||
                p.Brand.ToLower().Contains(keyword) ||
                p.ManufacturerCode.ToLower().Contains(keyword) ||
                (p.Description != null && p.Description.ToLower().Contains(keyword)) ||
                (p.SpecialCode1 != null && p.SpecialCode1.ToLower().Contains(keyword)) ||
                (p.SpecialCode2 != null && p.SpecialCode2.ToLower().Contains(keyword)));
        }

        return await query
            .OrderBy(p => p.Category!.Name)
            .ThenBy(p => p.ProductName)
            .ToListAsync();
    }

    public Task<Product?> GetByIdAsync(int id) =>
        _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

    public Task<List<Product>> GetSimilarByBrandAsync(Product product, int take = 4) =>
        _context.Products
            .AsNoTracking()
            .Where(p => p.Brand == product.Brand && p.Id != product.Id)
            .OrderBy(p => p.ProductName)
            .Take(take)
            .ToListAsync();

    public Task<List<Category>> GetCategoriesAsync() =>
        _context.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync();

    public async Task CreateAsync(Product product, IFormFile? image)
    {
        if (image is { Length: > 0 })
            product.ImageUrl = await UploadImageAsync(image);

        _context.Products.Add(product);
        await _context.SaveChangesAsync(); 
    }

    
    public async Task<bool> UpdateAsync(Product product, IFormFile? image)
    {
        if (!await _context.Products.AnyAsync(p => p.Id == product.Id))
            return false;

        var hasNewImage = image is { Length: > 0 };
        if (hasNewImage)
            product.ImageUrl = await UploadImageAsync(image!);

        _context.Products.Update(product);

      
        if (!hasNewImage)
            _context.Entry(product).Property(p => p.ImageUrl).IsModified = false;

        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<string> UploadImageAsync(IFormFile file)
    {
        await using var stream = file.OpenReadStream();

        var result = await _cloudinary.UploadAsync(new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = ImageFolder
        });

        if (result.Error != null)
            throw new InvalidOperationException($"Görsel yüklenemedi: {result.Error.Message}");

        return result.SecureUrl.AbsoluteUri;
    }
}