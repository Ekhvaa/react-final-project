using System.ComponentModel.DataAnnotations;

namespace TourApi.DTOs.Auth;

// Public sign-up is for tourists only. Employee accounts are created by admins.
public class RegisterRequest
{
    [Required]
    [StringLength(30, MinimumLength = 3)]
    public string Username { get; set; } = null!;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = null!;

    [Required]
    [EmailAddress]
    [StringLength(50)]
    public string Email { get; set; } = null!;

    [Required]
    [StringLength(20)]
    public string ContactPhone { get; set; } = null!;

    public DateTime DateOfBirth { get; set; }

    public char Gender { get; set; }

    [Required]
    [StringLength(20)]
    public string NationalId { get; set; } = null!;
}
