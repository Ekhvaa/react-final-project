namespace TourApi.Models;

public class TourDetail : BaseEntity
{
    public int TourId { get; set; }
    public Tour Tour { get; set; } = null!;

    public int CityId { get; set; }
    public City City { get; set; } = null!;

    public int? HotelId { get; set; }
    public Hotel? Hotel { get; set; }

    public byte Sequence { get; set; }
    public DateTime EstimatedArrivalDate { get; set; }
    public DateTime EstimatedDepartureDate { get; set; }
}
