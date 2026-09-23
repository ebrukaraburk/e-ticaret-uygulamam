namespace MiniB2B.Models;

public class ProfileViewModel
{
    public User User { get; set; } = null!;
    public SavedCard? SavedCard { get; set; }
}