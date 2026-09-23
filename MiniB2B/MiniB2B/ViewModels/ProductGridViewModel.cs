using MiniB2B.Models;

namespace MiniB2B.ViewModels
{
    public class ProductGridViewModel
    {
        public List<Product> Products { get; set; } = new();
        public List<GridColumnConfig> Columns { get; set; } = new();

        public string? AramaKelimesi { get; set; }
        public int? KategoriId { get; set; }
        public List<Category> Kategoriler { get; set; } = new();
        public HashSet<int> FeaturedIds { get; set; } = new();
        public bool IsAdmin { get; set; }
    }
}