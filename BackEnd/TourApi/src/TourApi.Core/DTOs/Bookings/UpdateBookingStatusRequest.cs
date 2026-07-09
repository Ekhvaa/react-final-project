using TourApi.Models;

namespace TourApi.DTOs.Bookings;

public class UpdateBookingStatusRequest
{
    public BookingStatus Status { get; set; }
}
