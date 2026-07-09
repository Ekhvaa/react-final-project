using TourApi.DTOs.Hotels;
using TourApi.Models;

namespace TourApi.Factories;

public interface IHotelFactory
{
    Hotel Create(HotelCreateRequest request);
    void Update(Hotel hotel, HotelUpdateRequest request);
    IEnumerable<HotelServiceMapping> CreateServiceMappings(int hotelId, IEnumerable<int> serviceIds);
}
