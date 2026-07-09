using System.ComponentModel.DataAnnotations;

namespace TourApi.DTOs.Auth;

public sealed class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    [StringLength(50)]
    public string Email { get; set; } = string.Empty;
}
