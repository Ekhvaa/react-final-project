using TourApi.DTOs.Hotels;
using TourApi.Models;

namespace TourApi.Factories;

public sealed class HotelFactory : IHotelFactory
{
    public Hotel Create(HotelCreateRequest request)
    {
        var hotel = new Hotel
        {
            Name = request.Name,
            StarRating = request.StarRating,
            CityId = request.CityId
        };

        foreach (var mapping in CreateServiceMappings(0, request.HotelServiceIds))
        {
            hotel.HotelServiceMappings.Add(mapping);
        }

        return hotel;
    }

    public void Update(Hotel hotel, HotelUpdateRequest request)
    {
        hotel.Name = request.Name;
        hotel.StarRating = request.StarRating;
        hotel.CityId = request.CityId;
        hotel.UpdateDate = DateTime.UtcNow;
    }

    public IEnumerable<HotelServiceMapping> CreateServiceMappings(int hotelId, IEnumerable<int> serviceIds) =>
        serviceIds
            .Distinct()
            .Select(serviceId => new HotelServiceMapping
            {
                HotelId = hotelId,
                HotelServiceId = serviceId
            });
}
