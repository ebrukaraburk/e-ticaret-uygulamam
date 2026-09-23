using Microsoft.EntityFrameworkCore;
using MiniB2B.Data;
using MiniB2B.Models;

namespace MiniB2B.Services
{
    public class OrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public static string? SonrakiDurum(string mevcut)
        {
            int i = Array.IndexOf(OrderStatus.Flow, mevcut);
            return i >= 0 && i < OrderStatus.Flow.Length - 1 ? OrderStatus.Flow[i + 1] : null;
        }

        // Rozet rengi (Bootstrap: bg-{renk})
        public static string Renk(string durum) => durum switch
        {
            OrderStatus.Pending => "warning text-dark",
            OrderStatus.Approved => "info text-dark",
            OrderStatus.Preparing or OrderStatus.Loaded or OrderStatus.Shipped or OrderStatus.OutForDelivery => "primary",
            OrderStatus.Delivered => "success",
            OrderStatus.Rejected => "danger",
            OrderStatus.Cancelled => "secondary",
            _ => "secondary"
        };

        // Yönetici butonunda görünecek yazı
        public static string ButonMetni(string sonrakiDurum) => sonrakiDurum switch
        {
            OrderStatus.Approved => "Onayla",
            OrderStatus.Preparing => "Hazırlanmaya Al",
            OrderStatus.Loaded => "Araca Yükle",
            OrderStatus.Shipped => "Kargoya Ver",
            OrderStatus.OutForDelivery => "Yola Çıkar",
            OrderStatus.Delivered => "Teslim Edildi Yap",
            _ => sonrakiDurum
        };

        public string SepeteUrunEkle(int userId, int productId, int adet)
        {
         
            if (adet <= 0)
                return "Geçersiz adet. Lütfen 1 veya daha fazla adet giriniz.";

            var product = _context.Products.Find(productId);
            if (product == null)
                return "Ürün bulunamadı.";

            var basket = GetOrCreateBasket(userId);
            var item = basket.Items.FirstOrDefault(i => i.ProductId == productId);
            int mevcutAdet = item?.Quantity ?? 0;
            int toplam = mevcutAdet + adet;

            if (toplam > product.StockQuantity)
                return $"Yetersiz stok! Sepetinizde zaten {mevcutAdet} adet var. Eklemek istediğinizle beraber toplam {toplam} oluyor ancak mevcut stok: {product.StockQuantity}.";

            if (item != null)
                item.Quantity += adet;
            else
                basket.Items.Add(new BasketItem
                {
                    BasketId = basket.Id,
                    ProductId = productId,
                    Quantity = adet,
                    UnitPrice = product.UnitPrice
                });

            _context.SaveChanges();
            return "Ürün başarıyla sepete eklendi!";
        }

        public string SepettenUrunCikar(int userId, int basketItemId)
        {
            var item = GetBasket(userId)?.Items.FirstOrDefault(i => i.Id == basketItemId);
            if (item == null)
                return "Ürün sepette bulunamadı.";

            _context.BasketItems.Remove(item);
            _context.SaveChanges();
            return "Ürün sepetten kaldırıldı.";
        }

        // Adet 0 veya altına inerse ürünü tamamen kaldırır.
        public string SepetUrunAdetGuncelle(int userId, int basketItemId, int delta)
        {
            var item = GetBasket(userId)?.Items.FirstOrDefault(i => i.Id == basketItemId);
            if (item == null)
                return "Ürün sepette bulunamadı.";

            int yeniAdet = item.Quantity + delta;
            if (yeniAdet <= 0)
            {
                _context.BasketItems.Remove(item);
                _context.SaveChanges();
                return "Ürün sepetten kaldırıldı.";
            }

            if (item.Product != null && yeniAdet > item.Product.StockQuantity)
                return $"Yetersiz stok! Mevcut stok: {item.Product.StockQuantity}.";

            item.Quantity = yeniAdet;
            _context.SaveChanges();
            return "Adet güncellendi.";
        }

        // Sadece "Onay Bekliyor" durumundaki siparişler iptal edilebilir;
        // iptal edilince ürünlerin stoğu geri yüklenir.
        public string SiparisIptalEt(int userId, int orderId)
        {
            var order = _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefault(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
                return "Sipariş bulunamadı.";

            if (order.Status != OrderStatus.Pending)
                return $"Bu sipariş '{order.Status}' durumunda olduğu için iptal edilemez.";

            foreach (var item in order.OrderItems)
            {
                var product = _context.Products.Find(item.ProductId);
                if (product != null)
                    product.StockQuantity += item.Quantity;
            }

            order.Status = OrderStatus.Cancelled;
            _context.SaveChanges();
            return "Siparişiniz başarıyla iptal edildi.";
        }

        // Sepetteki tüm ürünleri kontrol eder, stok yeterliyse siparişe dönüştürür
        public string SiparisOlusturFromBasket(int userId)
        {
            var basket = GetBasket(userId);
            if (basket == null || !basket.Items.Any())
                return "Sepetiniz boş, sipariş oluşturulamadı.";

            foreach (var item in basket.Items)
            {
                if (item.Product == null || item.Product.StockQuantity < item.Quantity)
                    return $"{item.Product?.ProductName ?? "Ürün"} için yeterli stok bulunmamaktadır. Mevcut stok: {item.Product?.StockQuantity ?? 0}.";
            }

            var orderItems = new List<OrderItem>();
            decimal totalAmount = 0;

            foreach (var item in basket.Items)
            {
                var product = item.Product!;
                product.StockQuantity -= item.Quantity;

                decimal lineTotal = product.UnitPrice * item.Quantity;
                totalAmount += lineTotal;

                orderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    ProductCode = product.ProductCode,
                    ProductName = product.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = product.UnitPrice,
                    TotalPrice = lineTotal
                });
            }

            _context.Orders.Add(new Order
            {
                UserId = userId,
                OrderNumber = $"ORD-{DateTime.Now:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}",
                OrderDate = DateTime.Now,
                Status = OrderStatus.Pending,
                TotalAmount = totalAmount,
                OrderItems = orderItems
            });

            _context.BasketItems.RemoveRange(basket.Items);
            _context.SaveChanges();
            return "Siparişiniz başarıyla oluşturuldu.";
        }

        // Yöneticinin siparişi bir sonraki aşamaya ilerletmesi
        public string SiparisSonrakiAsamayaGecir(int orderId)
        {
            var order = _context.Orders.Find(orderId);
            if (order == null)
                return "Sipariş bulunamadı.";

            if (order.Status is OrderStatus.Rejected or OrderStatus.Cancelled)
                return "Bu sipariş reddedildiği/iptal edildiği için ilerletilemez.";

            var sonraki = SonrakiDurum(order.Status);
            if (sonraki == null)
                return "Sipariş zaten son aşamada (Teslim Edildi).";

            order.Status = sonraki;
            _context.SaveChanges();
            return $"Sipariş durumu '{sonraki}' olarak güncellendi.";
        }

        // Sadece "Onay Bekliyor" durumundaki siparişler reddedilebilir
        public string SiparisReddet(int orderId)
        {
            var order = _context.Orders.Find(orderId);
            if (order == null)
                return "Sipariş bulunamadı.";

            if (order.Status != OrderStatus.Pending)
                return "Bu sipariş artık reddedilemez.";

            order.Status = OrderStatus.Rejected;
            _context.SaveChanges();
            return "Sipariş reddedildi.";
        }

        // --- Yardımcılar ---

        private Basket? GetBasket(int userId) =>
            _context.Baskets
                .Include(b => b.Items).ThenInclude(i => i.Product)
                .FirstOrDefault(b => b.UserId == userId);

        private Basket GetOrCreateBasket(int userId)
        {
            var basket = GetBasket(userId);
            if (basket != null)
                return basket;

            basket = new Basket { UserId = userId };
            _context.Baskets.Add(basket);
            _context.SaveChanges();
            return basket;
        }
    }
}