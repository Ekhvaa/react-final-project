namespace TourApi.DTOs.Geography;

public class CountryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string IsoName { get; set; } = null!;
    public string? FlagUrl { get; set; }
}
