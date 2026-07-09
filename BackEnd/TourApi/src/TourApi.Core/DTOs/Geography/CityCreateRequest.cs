using System.ComponentModel.DataAnnotations;

namespace TourApi.DTOs.Geography;

public class CityCreateRequest
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    public int CountryId { get; set; }
}
