using AutoMapper;
using TourApi.DTOs.Geography;
using TourApi.DTOs.Images;
using TourApi.Exceptions;
using TourApi.Models;
using TourApi.Repositories;

namespace TourApi.Services;

public class GeographyService : IGeographyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFileStorageService _fileStorageService;

    public GeographyService(IUnitOfWork unitOfWork, IMapper mapper, IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fileStorageService = fileStorageService;
    }

    public async Task<List<CountryDto>> GetCountriesAsync()
    {
        var countries = await _unitOfWork.Countries.ListActiveAsync();

        return _mapper.Map<List<CountryDto>>(countries);
    }

    public async Task<CountryDto?> GetCountryByIdAsync(int id)
    {
        var country = await _unitOfWork.Countries.GetActiveByIdAsync(id);

        return country is null ? null : _mapper.Map<CountryDto>(country);
    }

    public async Task<CountryDto> CreateCountryAsync(CountryCreateRequest request)
    {
        var country = _mapper.Map<Country>(request);

        _unitOfWork.Countries.Add(country);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CountryDto>(country);
    }

    public async Task<CountryDto> UploadCountryFlagAsync(
        int countryId,
        FileUpload file,
        CancellationToken cancellationToken = default)
    {
        var country = await _unitOfWork.Countries.GetActiveByIdAsync(countryId, cancellationToken);
        if (country is null)
            throw new ResourceNotFoundException("Country was not found.");

        var stored = await _fileStorageService.SaveImageAsync(file, "countries", cancellationToken);
        country.FlagUrl = stored.Url;
        country.UpdateDate = DateTime.UtcNow;

        _unitOfWork.Countries.Update(country);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CountryDto>(country);
    }

    public async Task<List<CityDto>> GetCitiesAsync(int? countryId)
    {
        var cities = await _unitOfWork.Cities.ListActiveWithCountryAsync(countryId);

        return _mapper.Map<List<CityDto>>(cities);
    }

    public async Task<CityDto?> GetCityByIdAsync(int id)
    {
        var city = await _unitOfWork.Cities.GetActiveWithCountryByIdAsync(id);

        return city is null ? null : _mapper.Map<CityDto>(city);
    }

    public async Task<CityDto> CreateCityAsync(CityCreateRequest request)
    {
        var countryExists = await _unitOfWork.Countries.ExistsAsync(c => c.Id == request.CountryId && !c.IsDeleted);
        if (!countryExists)
            throw new InvalidOperationException("Country not found.");

        var city = _mapper.Map<City>(request);

        _unitOfWork.Cities.Add(city);
        await _unitOfWork.SaveChangesAsync();

        return (await GetCityByIdAsync(city.Id))!;
    }
}
