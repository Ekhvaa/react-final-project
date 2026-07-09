namespace TourApi.DTOs.Tours;

public class TourItineraryLegCreateRequest
{
    public byte Sequence { get; set; }
    public int CityId { get; set; }
    public int? HotelId { get; set; }
    public DateTime EstimatedArrivalDate { get; set; }
    public DateTime EstimatedDepartureDate { get; set; }
}
