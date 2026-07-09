using System.ComponentModel.DataAnnotations;

namespace TourApi.DTOs.Auth;

public sealed class GoogleLoginRequest
{
    [Required]
    public string IdToken { get; set; } = string.Empty;
}
