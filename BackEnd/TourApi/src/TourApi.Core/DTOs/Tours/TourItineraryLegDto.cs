namespace TourApi.DTOs.Tours;

public class TourItineraryLegDto
{
    public int Id { get; set; }
    public byte Sequence { get; set; }
    public int CityId { get; set; }
    public string CityName { get; set; } = null!;
    public int? HotelId { get; set; }
    public string? HotelName { get; set; }
    public DateTime EstimatedArrivalDate { get; set; }
    public DateTime EstimatedDepartureDate { get; set; }
}
