namespace TourApi.Models;

public class Country : BaseEntity
{
    public string Name { get; set; } = null!;
    public string IsoName { get; set; } = null!;
    public string? FlagUrl { get; set; }

    public ICollection<City> Cities { get; set; } = new List<City>();
}
