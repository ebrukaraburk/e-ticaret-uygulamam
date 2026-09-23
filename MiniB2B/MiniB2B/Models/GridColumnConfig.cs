namespace MiniB2B.Models
{
    public class GridColumnConfig
    {
        public int Id { get; set; }
        public string PropertyName { get; set; } = string.Empty; // Örn: ProductCode, StockQuantity
        public string ColumnTitle { get; set; } = string.Empty;  // Örn: Ürün Kodu, Stok Durumu
        public int DisplayOrder { get; set; }                    // Gösterim sırası (1, 2, 3...)
        public bool IsVisible { get; set; } = true;               // Görünsün mü?
        public string RenderType { get; set; } = "Text";          // Text, Image, Badge, Currency, Input, Action
        public bool AdminOnly { get; set; } = false;              // Sadece yönetici görsün mü?

        public string? ColumnWidth { get; set; }                  // "80px", "15%" vb.
        public string Alignment { get; set; } = "Left";           // "Left" | "Center" | "Right"
        public bool VisibleOnDesktop { get; set; } = true;
        public bool VisibleOnTablet { get; set; } = true;
        public bool VisibleOnMobile { get; set; } = true;
    }
}