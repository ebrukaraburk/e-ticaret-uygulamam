using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MiniB2B.Data;
using MiniB2B.Models;

namespace MiniB2B.Services;

public class UserService
{
    public const int MinPasswordLength = 6;

    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<User>> GetAllAsync() =>
        _context.Users.AsNoTracking().ToListAsync();

    public Task<User?> GetByIdAsync(int id) =>
        _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

    // Kullanıcı adı veya e-posta ile giriş; bilgiler yanlışsa null döner
    public async Task<User?> AuthenticateAsync(string usernameOrEmail, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail);

        if (user == null || !IsPasswordCorrect(user, password))
            return null;

        return user;
    }

    // excludeId: düzenlerken kullanıcının kendi kaydı çakışma sayılmasın diye
    public Task<bool> ExistsAsync(string username, string email, int excludeId = 0) =>
        _context.Users.AnyAsync(u => u.Id != excludeId && (u.Username == username || u.Email == email));

    public async Task CreateAsync(User user, string password)
    {
        SetPassword(user, password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    // Kullanıcı bulunamazsa false döner
    public async Task<bool> UpdateAsync(User updatedUser, string? newPassword)
    {
        var user = await _context.Users.FindAsync(updatedUser.Id);
        if (user == null) return false;

        user.FirstName = updatedUser.FirstName;
        user.LastName = updatedUser.LastName;
        user.Email = updatedUser.Email;
        user.PhoneNumber = updatedUser.PhoneNumber;
        user.Username = updatedUser.Username;
        user.Role = updatedUser.Role;

        // Şifre kutusu boşsa mevcut şifre değişmez
        if (!string.IsNullOrWhiteSpace(newPassword))
            SetPassword(user, newPassword);

        await _context.SaveChangesAsync();
        return true;
    }

    // Rastgele salt üretir, şifreyi bu salt ile hash'ler
    private static void SetPassword(User user, string password)
    {
        using var hmac = new HMACSHA512();
        user.PasswordSalt = hmac.Key;
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    private static bool IsPasswordCorrect(User user, string password)
    {
        using var hmac = new HMACSHA512(user.PasswordSalt);
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return CryptographicOperations.FixedTimeEquals(hash, user.PasswordHash);
    }





    // --- Adres ---

    public async Task SaveAddressAsync(int userId, string? addressLine, string? city, string? postalCode)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return;

        user.AddressLine = addressLine;
        user.City = city;
        user.PostalCode = postalCode;

        await _context.SaveChangesAsync();
    }

    // --- Kart (SADECE maskelenmiş bilgi: tam numara ve CVV asla parametre olarak alınmaz/saklanmaz) ---

    public async Task SaveCardAsync(int userId, string cardHolder, string last4, string expiryMonth, string expiryYear)
    {
        // Aynı kullanıcı için önceki kayıtlı kartı güncelle 
        var existing = await _context.SavedCards.FirstOrDefaultAsync(c => c.UserId == userId);

        if (existing != null)
        {
            existing.CardHolder = cardHolder;
            existing.Last4 = last4;
            existing.ExpiryMonth = expiryMonth;
            existing.ExpiryYear = expiryYear;
            existing.SavedAt = DateTime.Now;
        }
        else
        {
            _context.SavedCards.Add(new SavedCard
            {
                UserId = userId,
                CardHolder = cardHolder,
                Last4 = last4,
                ExpiryMonth = expiryMonth,
                ExpiryYear = expiryYear
            });
        }

        await _context.SaveChangesAsync();
    }

    public Task<SavedCard?> GetSavedCardAsync(int userId) =>
        _context.SavedCards.AsNoTracking().FirstOrDefaultAsync(c => c.UserId == userId);
}