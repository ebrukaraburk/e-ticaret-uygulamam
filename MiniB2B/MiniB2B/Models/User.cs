using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MiniB2B.Models;

public class User
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ad zorunludur.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Soyad zorunludur.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    // --- Adres bilgileri ---
    public string? AddressLine { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }

    // Formdan gelmez, sadece serviste doldurulur
    [BindNever, ValidateNever]
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();

    [BindNever, ValidateNever]
    public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();

    public string Role { get; set; } = "Customer"; // "Admin" veya "Customer"
}