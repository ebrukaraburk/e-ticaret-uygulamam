namespace MiniB2B.Models;

public class SavedCard
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }

    public string CardHolder { get; set; } = string.Empty;

    // Sadece son 4 hane saklanır — tam numara ASLA kaydedilmez
    public string Last4 { get; set; } = string.Empty;

    public string ExpiryMonth { get; set; } = string.Empty;
    public string ExpiryYear { get; set; } = string.Empty;

    public DateTime SavedAt { get; set; } = DateTime.Now;
}