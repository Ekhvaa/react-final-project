using System.ComponentModel.DataAnnotations;

namespace TourApi.DTOs.Tours;

public class TourCreateRequest
{
    [Required]
    [StringLength(50)]
    public string Code { get; set; } = null!;

    [Required]
    public string Name { get; set; } = null!;

    [Range(0.01, 999999999)]
    public decimal CurrentPrice { get; set; }

    [MinLength(1)]
    public List<TourItineraryLegCreateRequest> Itinerary { get; set; } = new();
}
