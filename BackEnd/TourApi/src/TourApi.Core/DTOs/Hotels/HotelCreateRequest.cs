using System.ComponentModel.DataAnnotations;

namespace TourApi.DTOs.Hotels;

public class HotelCreateRequest
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Range(1, 5)]
    public byte StarRating { get; set; }

    public int CityId { get; set; }
    public List<int> HotelServiceIds { get; set; } = new();
}
