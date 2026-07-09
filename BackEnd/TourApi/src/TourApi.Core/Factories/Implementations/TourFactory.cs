using TourApi.DTOs.Tours;
using TourApi.Models;

namespace TourApi.Factories;

public sealed class TourFactory : ITourFactory
{
    public Tour Create(TourCreateRequest request)
    {
        var tour = new Tour
        {
            Code = request.Code,
            Name = request.Name,
            CurrentPrice = request.CurrentPrice,
            TourDetailId = 0
        };

        foreach (var leg in CreateItinerary(0, request.Itinerary))
        {
            tour.TourDetails.Add(leg);
        }

        return tour;
    }

    public void Update(Tour tour, TourUpdateRequest request)
    {
        tour.Code = request.Code;
        tour.Name = request.Name;
        tour.CurrentPrice = request.CurrentPrice;
        tour.UpdateDate = DateTime.UtcNow;
    }

    public IEnumerable<TourDetail> CreateItinerary(int tourId, IEnumerable<TourItineraryLegCreateRequest> itinerary) =>
        itinerary
            .OrderBy(leg => leg.Sequence)
            .Select(leg => new TourDetail
            {
                TourId = tourId,
                CityId = leg.CityId,
                HotelId = leg.HotelId,
                Sequence = leg.Sequence,
                EstimatedArrivalDate = leg.EstimatedArrivalDate,
                EstimatedDepartureDate = leg.EstimatedDepartureDate
            });
}
