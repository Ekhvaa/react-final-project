using System.ComponentModel.DataAnnotations;

namespace TourApi.DTOs.Geography;

public class CountryCreateRequest
{
    [Required]
    [StringLength(30)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(2, MinimumLength = 2)]
    public string IsoName { get; set; } = null!;
}
