namespace TourApi.Models;

public class User : BaseEntity
{
    public string Username { get; set; } = null!;

    // SHA-512 hash (64 bytes) - see AuthService for hashing implementation.
    // External-login users get a random unusable hash so they cannot log in
    // through the local password endpoint unless password-reset is added later.
    public byte[] PasswordHash { get; set; } = null!;
    public bool EmailConfirmed { get; set; }

    public Employee? Employee { get; set; }
    public Tourist? Tourist { get; set; }
    public ICollection<ExternalLogin> ExternalLogins { get; set; } = new List<ExternalLogin>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<EmailConfirmationToken> EmailConfirmationTokens { get; set; } = new List<EmailConfirmationToken>();
    public ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();
}
