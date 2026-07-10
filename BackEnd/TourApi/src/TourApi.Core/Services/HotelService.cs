using AutoMapper;
using TourApi.DTOs.Hotels;
using TourApi.Exceptions;
using TourApi.Factories;
using TourApi.Models;
using TourApi.Repositories;

namespace TourApi.Services;

public class HotelService : IHotelService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHotelFactory _hotelFactory;

    public HotelService(IUnitOfWork unitOfWork, IMapper mapper, IHotelFactory hotelFactory)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _hotelFactory = hotelFactory;
    }

    public async Task<List<HotelDto>> GetHotelsAsync(int? cityId)
    {
        var hotels = await _unitOfWork.Hotels.ListActiveWithDetailsAsync(cityId);

        return _mapper.Map<List<HotelDto>>(hotels);
    }

    public async Task<HotelDto?> GetHotelByIdAsync(int id)
    {
        var hotel = await _unitOfWork.Hotels.GetActiveWithDetailsByIdAsync(id);
        return hotel is null ? null : _mapper.Map<HotelDto>(hotel);
    }

    public async Task<HotelDto> CreateHotelAsync(HotelCreateRequest request)
    {
        await ValidateHotelRequestAsync(request.CityId, request.HotelServiceIds);

        var hotel = _hotelFactory.Create(request);

        _unitOfWork.Hotels.Add(hotel);
        await _unitOfWork.SaveChangesAsync();

        return (await GetHotelByIdAsync(hotel.Id))!;
    }

    public async Task<bool> UpdateHotelAsync(int id, HotelUpdateRequest request)
    {
        var hotel = await _unitOfWork.Hotels.GetActiveForUpdateAsync(id);

        if (hotel is null) return false;

        await ValidateHotelRequestAsync(request.CityId, request.HotelServiceIds);

        _hotelFactory.Update(hotel, request);

        SynchronizeHotelServices(hotel, request.HotelServiceIds);

        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<List<HotelServiceDto>> GetHotelServicesAsync()
    {
        var services = await _unitOfWork.HotelServices.ListOrderedAsync();

        return _mapper.Map<List<HotelServiceDto>>(services);
    }

    public async Task<HotelServiceDto> CreateHotelServiceAsync(HotelServiceCreateRequest request)
    {
        var serviceName = request.Name.Trim();

        if (string.IsNullOrWhiteSpace(serviceName))
        {
            throw new InvalidOperationException("Hotel service name is required.");
        }

        var alreadyExists = await _unitOfWork.HotelServices.ExistsByNameAsync(serviceName);

        if (alreadyExists)
        {
            throw new DuplicateResourceException("Hotel service already exists.");
        }

        var service = new TourApi.Models.HotelService
        {
            Name = serviceName
        };

        _unitOfWork.HotelServices.Add(service);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<HotelServiceDto>(service);
    }

    private void SynchronizeHotelServices(Hotel hotel, IEnumerable<int> requestedServiceIds)
    {
        var desiredServiceIds = requestedServiceIds.Distinct().ToHashSet();
        var mappingsToRemove = hotel.HotelServiceMappings
            .Where(mapping => !desiredServiceIds.Contains(mapping.HotelServiceId))
            .ToList();

        _unitOfWork.HotelServiceMappings.RemoveRange(mappingsToRemove);

        var existingServiceIds = hotel.HotelServiceMappings
            .Select(mapping => mapping.HotelServiceId)
            .ToHashSet();

        var mappingsToAdd = desiredServiceIds
            .Except(existingServiceIds)
            .Select(serviceId => new HotelServiceMapping
            {
                HotelId = hotel.Id,
                HotelServiceId = serviceId
            });

        _unitOfWork.HotelServiceMappings.AddRange(mappingsToAdd);
    }

    private async Task ValidateHotelRequestAsync(int cityId, IEnumerable<int> serviceIds)
    {
        var cityExists = await _unitOfWork.Cities.ExistsAsync(c => c.Id == cityId && !c.IsDeleted);
        if (!cityExists)
            throw new InvalidOperationException("City not found.");

        var serviceIdList = serviceIds.Distinct().ToList();
        if (serviceIdList.Count == 0) return;

        var existingServiceCount = await _unitOfWork.HotelServices.CountExistingByIdsAsync(serviceIdList);

        if (existingServiceCount != serviceIdList.Count)
            throw new InvalidOperationException("One or more hotel services were not found.");
    }
}
