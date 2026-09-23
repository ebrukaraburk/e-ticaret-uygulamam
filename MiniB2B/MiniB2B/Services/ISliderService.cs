using Microsoft.AspNetCore.Http;
using MiniB2B.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniB2B.Services
{
    public interface ISliderService
    {
        Task<List<SliderItem>> GetAllSlidersAsync();
        Task<List<SliderItem>> GetActiveSlidersAsync();
        Task<SliderItem?> GetByIdAsync(int id);
        Task AddSliderAsync(SliderItem slider, IFormFile? imageFile);
        Task DeleteSliderAsync(int id);

        Task<HashSet<int>> GetFeaturedProductIdsAsync();
        Task<bool> IsFeaturedAsync(int productId);
        Task RemoveByProductIdAsync(int productId);
    }
}