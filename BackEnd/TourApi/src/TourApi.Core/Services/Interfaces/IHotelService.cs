using TourApi.DTOs.Hotels;

namespace TourApi.Services;

public interface IHotelService
{
    Task<List<HotelDto>> GetHotelsAsync(int? cityId);
    Task<HotelDto?> GetHotelByIdAsync(int id);
    Task<HotelDto> CreateHotelAsync(HotelCreateRequest request);
    Task<bool> UpdateHotelAsync(int id, HotelUpdateRequest request);
    Task<List<HotelServiceDto>> GetHotelServicesAsync();
    Task<HotelServiceDto> CreateHotelServiceAsync(HotelServiceCreateRequest request);
}