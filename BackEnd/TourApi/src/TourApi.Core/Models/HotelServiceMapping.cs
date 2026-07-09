namespace TourApi.Models;

public class HotelServiceMapping
{
    public int HotelId { get; set; }
    public Hotel Hotel { get; set; } = null!;

    public int HotelServiceId { get; set; }
    public HotelService HotelService { get; set; } = null!;
}
