namespace MiniB2B.Models
{
   
    public static class OrderStatus
    {
        public const string Pending = "Onay Bekliyor";
        public const string Approved = "Onaylandı";
        public const string Preparing = "Hazırlanıyor";
        public const string Loaded = "Araca Yüklendi";
        public const string Shipped = "Kargoya Verildi";
        public const string OutForDelivery = "Yola Çıktı";
        public const string Delivered = "Teslim Edildi";
        public const string Rejected = "Reddedildi";
        public const string Cancelled = "İptal Edildi";

        // Sipariş akışı . Adım eklemek/çıkarmak
        public static readonly string[] Flow =
        {
            Pending, Approved, Preparing, Loaded, Shipped, OutForDelivery, Delivered
        };
    }
}