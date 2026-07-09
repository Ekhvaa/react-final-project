namespace TourApi.DTOs.Geography;

public class CityDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int CountryId { get; set; }
    public string CountryName { get; set; } = null!;
}
