using Microsoft.EntityFrameworkCore;
using MiniB2B.Models;

namespace MiniB2B.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<GridColumnConfig> GridColumnConfigs { get; set; }
        public DbSet<SliderItem> SliderItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SavedCard> SavedCards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.ProductCode)
                .IsUnique();

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.OrderNumber)
                .IsUnique();

            modelBuilder.Entity<Product>()
                .Property(p => p.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<BasketItem>()
                .Property(bi => bi.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.TotalPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Basket>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Category>().HasIndex(c => c.Name).IsUnique();

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Telefon" },
                new Category { Id = 2, Name = "Bilgisayar" },
                new Category { Id = 3, Name = "Tablet" },
                new Category { Id = 4, Name = "Aksesuar" },
                new Category { Id = 5, Name = "Televizyon" },
                new Category { Id = 6, Name = "Ses Sistemleri" },
                new Category { Id = 7, Name = "Giyilebilir Teknoloji" },
                new Category { Id = 8, Name = "Gaming" },
                new Category { Id = 9, Name = "Ağ Ürünleri" },
                new Category { Id = 10, Name = "Yazıcı ve Tarayıcı" },
                new Category { Id = 11, Name = "Depolama" },
                new Category { Id = 12, Name = "Beyaz Eşya" },
                new Category { Id = 13, Name = "Akıllı Ev" },
                new Category { Id = 14, Name = "Ofis Elektroniği" }
            );

            modelBuilder.Entity<GridColumnConfig>().HasData(
                new GridColumnConfig { Id = 1, PropertyName = "ImageUrl", ColumnTitle = "Görsel", DisplayOrder = 1, RenderType = "Image", IsVisible = true, AdminOnly = false, ColumnWidth = "70px", Alignment = "Center", VisibleOnDesktop = true, VisibleOnTablet = true, VisibleOnMobile = true },
                new GridColumnConfig { Id = 2, PropertyName = "ProductCode", ColumnTitle = "Ürün Kodu", DisplayOrder = 2, RenderType = "Text", IsVisible = true, AdminOnly = false, ColumnWidth = "110px", Alignment = "Left", VisibleOnDesktop = true, VisibleOnTablet = true, VisibleOnMobile = false },
                new GridColumnConfig { Id = 3, PropertyName = "ProductName", ColumnTitle = "Ürün Adı", DisplayOrder = 3, RenderType = "Text", IsVisible = true, AdminOnly = false, ColumnWidth = "25%", Alignment = "Left", VisibleOnDesktop = true, VisibleOnTablet = true, VisibleOnMobile = true },
                new GridColumnConfig { Id = 4, PropertyName = "Brand", ColumnTitle = "Marka", DisplayOrder = 4, RenderType = "Text", IsVisible = true, AdminOnly = false, ColumnWidth = "120px", Alignment = "Left", VisibleOnDesktop = true, VisibleOnTablet = true, VisibleOnMobile = false },
                new GridColumnConfig { Id = 5, PropertyName = "ManufacturerCode", ColumnTitle = "Üretici Kodu", DisplayOrder = 5, RenderType = "Text", IsVisible = true, AdminOnly = false, ColumnWidth = "120px", Alignment = "Left", VisibleOnDesktop = true, VisibleOnTablet = false, VisibleOnMobile = false },
                new GridColumnConfig { Id = 6, PropertyName = "SpecialCode1", ColumnTitle = "Özel Kod 1", DisplayOrder = 6, RenderType = "Text", IsVisible = true, AdminOnly = true, ColumnWidth = "100px", Alignment = "Left", VisibleOnDesktop = true, VisibleOnTablet = false, VisibleOnMobile = false },
                new GridColumnConfig { Id = 7, PropertyName = "SpecialCode2", ColumnTitle = "Özel Kod 2", DisplayOrder = 7, RenderType = "Text", IsVisible = true, AdminOnly = true, ColumnWidth = "100px", Alignment = "Left", VisibleOnDesktop = true, VisibleOnTablet = false, VisibleOnMobile = false },
                new GridColumnConfig { Id = 8, PropertyName = "StockQuantity", ColumnTitle = "Stok Durumu", DisplayOrder = 8, RenderType = "Badge", IsVisible = true, AdminOnly = false, ColumnWidth = "110px", Alignment = "Center", VisibleOnDesktop = true, VisibleOnTablet = true, VisibleOnMobile = true },
                new GridColumnConfig { Id = 9, PropertyName = "UnitPrice", ColumnTitle = "Birim Fiyat", DisplayOrder = 9, RenderType = "Currency", IsVisible = true, AdminOnly = false, ColumnWidth = "110px", Alignment = "Right", VisibleOnDesktop = true, VisibleOnTablet = true, VisibleOnMobile = true },
                new GridColumnConfig { Id = 10, PropertyName = "Id", ColumnTitle = "Sipariş", DisplayOrder = 10, RenderType = "Input", IsVisible = true, AdminOnly = false, ColumnWidth = "130px", Alignment = "Center", VisibleOnDesktop = true, VisibleOnTablet = true, VisibleOnMobile = true },
                new GridColumnConfig { Id = 11, PropertyName = "Id", ColumnTitle = "İşlem", DisplayOrder = 11, RenderType = "Action", IsVisible = true, AdminOnly = false, ColumnWidth = "160px", Alignment = "Right", VisibleOnDesktop = true, VisibleOnTablet = true, VisibleOnMobile = true }
            );
        }
    }
}