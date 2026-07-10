using TourApi.DTOs.Images;

namespace TourApi.DTOs.Tours;

public class TourSummaryDto
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal CurrentPrice { get; set; }
    public string? StartingCity { get; set; }
    public string? StartingCountry { get; set; }
    public DateTime? EarliestDeparture { get; set; }
    public int DurationDays { get; set; }
    public int? AssignedTourGuideId { get; set; }
    public string? AssignedTourGuideFullName { get; set; }
    public List<ImageDto> Images { get; set; } = new();
}