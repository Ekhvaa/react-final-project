namespace TourApi.Models;

public class Hotel : BaseEntity
{
    public int CityId { get; set; }
    public City City { get; set; } = null!;

    public string Name { get; set; } = null!;
    public byte StarRating { get; set; }

    public ICollection<HotelServiceMapping> HotelServiceMappings { get; set; } = new List<HotelServiceMapping>();
    public ICollection<TourDetail> TourDetails { get; set; } = new List<TourDetail>();
    public ICollection<HotelImage> Images { get; set; } = new List<HotelImage>();
}
