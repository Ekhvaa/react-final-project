using System.ComponentModel.DataAnnotations;

namespace TourApi.DTOs.Auth;

public sealed class ResetPasswordRequest
{
    [Required]
    public string Token { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    [StringLength(100)]
    public string NewPassword { get; set; } = string.Empty;
}
