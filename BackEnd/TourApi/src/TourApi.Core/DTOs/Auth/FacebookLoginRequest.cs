using System.ComponentModel.DataAnnotations;

namespace TourApi.DTOs.Auth;

public sealed class FacebookLoginRequest
{
    [Required]
    public string AccessToken { get; set; } = string.Empty;
}
