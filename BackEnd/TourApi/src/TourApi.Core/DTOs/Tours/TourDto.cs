using TourApi.DTOs.Images;

namespace TourApi.DTOs.Tours;

public class TourDto
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal CurrentPrice { get; set; }
    public int? AssignedTourGuideId { get; set; }
    public string? AssignedTourGuideFullName { get; set; }
    public int? AssignedTravelAgentId { get; set; }
    public string? AssignedTravelAgentFullName { get; set; }

    public List<TourItineraryLegDto> Itinerary { get; set; } = new();
    public List<ImageDto> Images { get; set; } = new();
}