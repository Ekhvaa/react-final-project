using TourApi.DTOs.Geography;
using TourApi.DTOs.Images;

namespace TourApi.Services;

public interface IGeographyService
{
    Task<List<CountryDto>> GetCountriesAsync();
    Task<CountryDto?> GetCountryByIdAsync(int id);
    Task<CountryDto> CreateCountryAsync(CountryCreateRequest request);
    Task<CountryDto> UploadCountryFlagAsync(int countryId, FileUpload file, CancellationToken cancellationToken = default);

    Task<List<CityDto>> GetCitiesAsync(int? countryId);
    Task<CityDto?> GetCityByIdAsync(int id);
    Task<CityDto> CreateCityAsync(CityCreateRequest request);
}
