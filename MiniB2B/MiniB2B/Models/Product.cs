using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniB2B.Models;

public class Product
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ürün kodu zorunludur.")]
    [StringLength(50)]
    [Display(Name = "Ürün Kodu")]
    public string ProductCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ürün adı zorunludur.")]
    [StringLength(200)]
    [Display(Name = "Ürün Adı")]
    public string ProductName { get; set; } = string.Empty;

    [StringLength(2000)]
    [Display(Name = "Açıklama")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Marka alanı zorunludur.")]
    [StringLength(100)]
    [Display(Name = "Marka")]
    public string Brand { get; set; } = string.Empty;

    [Required(ErrorMessage = "Üretici kodu zorunludur.")]
    [StringLength(50)]
    [Display(Name = "Üretici Kodu")]
    public string ManufacturerCode { get; set; } = string.Empty;

    [StringLength(50)]
    [Display(Name = "Özel Kod 1")]
    public string? SpecialCode1 { get; set; }

    [StringLength(50)]
    [Display(Name = "Özel Kod 2")]
    public string? SpecialCode2 { get; set; }

    [Display(Name = "Ürün Resmi URL")]
    public string? ImageUrl { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Stok 0 veya daha büyük olmalıdır.")]
    [Display(Name = "Stok Miktarı")]
    public int StockQuantity { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Kritik stok 0 veya daha büyük olmalıdır.")]
    [Display(Name = "Kritik Stok Seviyesi")]
    public int CriticalStockQuantity { get; set; } = 5;

    [Range(0.01, 999999999.99, ErrorMessage = "Fiyat 0'dan büyük olmalıdır.")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Birim Fiyat")]
    public decimal UnitPrice { get; set; }

    [Display(Name = "Kategori")]
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
}