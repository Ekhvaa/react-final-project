using System.ComponentModel.DataAnnotations;

namespace TourApi.DTOs.Users;

public class TouristUpdateRequest
{
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
}
