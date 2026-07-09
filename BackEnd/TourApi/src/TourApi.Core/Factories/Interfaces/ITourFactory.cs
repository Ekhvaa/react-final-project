using TourApi.DTOs.Tours;
using TourApi.Models;

namespace TourApi.Factories;

public interface ITourFactory
{
    Tour Create(TourCreateRequest request);
    void Update(Tour tour, TourUpdateRequest request);
    IEnumerable<TourDetail> CreateItinerary(int tourId, IEnumerable<TourItineraryLegCreateRequest> itinerary);
}
