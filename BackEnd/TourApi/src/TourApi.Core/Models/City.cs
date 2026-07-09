namespace TourApi.Models;

public class City : BaseEntity
{
    public int CountryId { get; set; }
    public Country Country { get; set; } = null!;

    public string Name { get; set; } = null!;

    public ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
    public ICollection<TourDetail> TourDetails { get; set; } = new List<TourDetail>();
}
