using MiniB2B.Models;

namespace MiniB2B.ViewModels
{
    public class GridCellViewModel
    {
        public GridColumnConfig Column { get; set; } = null!;
        public Product Product { get; set; } = null!;
        public bool IsFeatured { get; set; }
        public bool IsAdmin { get; set; }
    }
}