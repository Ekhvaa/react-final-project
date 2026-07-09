using TourApi.DTOs.Bookings;
using TourApi.Models;

namespace TourApi.Factories;

public sealed class BookingFactory : IBookingFactory
{
    public Booking Create(int touristId, CreateBookingRequest request, decimal currentTourPrice) => new()
    {
        TourId = request.TourId,
        TouristId = touristId,
        TravelAgentId = request.TravelAgentId,
        DateOfBooking = DateTime.UtcNow,
        PricePaid = currentTourPrice,
        Status = BookingStatus.Pending
    };
}
