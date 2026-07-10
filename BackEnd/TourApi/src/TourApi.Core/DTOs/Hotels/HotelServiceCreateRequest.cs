using System.ComponentModel.DataAnnotations;

namespace TourApi.DTOs.Hotels;

public class HotelServiceCreateRequest
{
    [Required]
    [StringLength(30)]
    public string Name { get; set; } = null!;
}