using System.ComponentModel.DataAnnotations;

namespace TourApi.DTOs.Auth;

public sealed class ConfirmEmailRequest
{
    [Required]
    public string Token { get; set; } = string.Empty;
}
