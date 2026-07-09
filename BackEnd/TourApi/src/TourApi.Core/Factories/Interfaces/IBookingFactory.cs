using TourApi.DTOs.Bookings;
using TourApi.Models;

namespace TourApi.Factories;

public interface IBookingFactory
{
    Booking Create(int touristId, CreateBookingRequest request, decimal currentTourPrice);
}
