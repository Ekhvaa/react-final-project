using TourApi.DTOs.Images;

namespace TourApi.DTOs.Hotels;

public class HotelDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public byte StarRating { get; set; }
    public int CityId { get; set; }
    public string CityName { get; set; } = null!;
    public string CountryName { get; set; } = null!;
    public List<HotelServiceDto> Services { get; set; } = new();
    public List<ImageDto> Images { get; set; } = new();
}
