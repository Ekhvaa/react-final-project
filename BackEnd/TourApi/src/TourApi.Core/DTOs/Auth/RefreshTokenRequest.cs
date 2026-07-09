using System.ComponentModel.DataAnnotations;

namespace TourApi.DTOs.Auth;

public sealed class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
