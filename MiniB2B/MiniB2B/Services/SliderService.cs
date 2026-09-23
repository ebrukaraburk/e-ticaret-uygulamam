using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MiniB2B.Data;
using MiniB2B.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MiniB2B.Services
{
    public class SliderService : ISliderService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SliderService(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<List<SliderItem>> GetAllSlidersAsync()
        {
            return await _context.SliderItems
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync();
        }

        public async Task<List<SliderItem>> GetActiveSlidersAsync()
        {
            return await _context.SliderItems
                .Where(x => x.IsActive)
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync();
        }

        public async Task<SliderItem?> GetByIdAsync(int id)
        {
            return await _context.SliderItems.FindAsync(id);
        }

        public async Task AddSliderAsync(SliderItem slider, IFormFile? imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "images", "slider");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                slider.ImageUrl = "/images/slider/" + uniqueFileName;
            }

            _context.SliderItems.Add(slider);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSliderAsync(int id)
        {
            var slider = await _context.SliderItems.FindAsync(id);
            if (slider == null) return;


            if (!string.IsNullOrEmpty(slider.ImageUrl) && slider.ImageUrl.StartsWith("/images/slider/"))
            {
                var filePath = Path.Combine(_env.WebRootPath, slider.ImageUrl.TrimStart('/'));
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }

            _context.SliderItems.Remove(slider);
            await _context.SaveChangesAsync();
        }

        // ---- Öne çıkarma ile ilgili metotlar ----

        public async Task<HashSet<int>> GetFeaturedProductIdsAsync()
        {
            var ids = await _context.SliderItems
                .Where(x => x.ProductId != null)
                .Select(x => x.ProductId!.Value)
                .ToListAsync();

            return ids.ToHashSet();
        }

        public async Task<bool> IsFeaturedAsync(int productId)
        {
            return await _context.SliderItems.AnyAsync(x => x.ProductId == productId);
        }

        public async Task RemoveByProductIdAsync(int productId)
        {
            var items = await _context.SliderItems
                .Where(x => x.ProductId == productId)
                .ToListAsync();

            if (items.Any())
            {
                _context.SliderItems.RemoveRange(items);
                await _context.SaveChangesAsync();
            }
        }
    }
}