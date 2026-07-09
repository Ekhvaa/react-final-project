namespace TourApi.Models;

public sealed class PasswordResetToken : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public string TokenHash { get; set; } = null!;
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? UsedAtUtc { get; set; }

    public bool IsActive => UsedAtUtc is null && ExpiresAtUtc > DateTime.UtcNow && !IsDeleted;
}
