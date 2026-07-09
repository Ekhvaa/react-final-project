using System.ComponentModel.DataAnnotations;

namespace TourApi.DTOs.Auth;

public sealed class RevokeTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
