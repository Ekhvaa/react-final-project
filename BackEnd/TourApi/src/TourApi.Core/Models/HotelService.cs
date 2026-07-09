namespace TourApi.Models;

public class HotelService
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<HotelServiceMapping> HotelServiceMappings { get; set; } = new List<HotelServiceMapping>();
}
