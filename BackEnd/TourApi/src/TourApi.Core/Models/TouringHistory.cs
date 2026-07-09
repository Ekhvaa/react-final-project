namespace TourApi.Models;

public class TouringHistory
{
    public int TouristId { get; set; }
    public Tourist Tourist { get; set; } = null!;

    public int TourId { get; set; }
    public Tour Tour { get; set; } = null!;

    public DateTime DepartureDate { get; set; }
    public DateTime ReturnDate { get; set; }
}
