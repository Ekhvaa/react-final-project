namespace TourApi.DTOs.Auth;

public class AuthResponse
{
    public string Token { get; set; } = null!;
    public DateTime ExpiresAtUtc { get; set; }
    public string RefreshToken { get; set; } = null!;
    public DateTime RefreshTokenExpiresAtUtc { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = null!;
    public string Role { get; set; } = null!;
    public bool EmailConfirmed { get; set; }
}
