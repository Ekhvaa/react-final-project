namespace TourApi.DTOs.Bookings;

// TouristId comes from the authenticated user's token, not the request body.
public class CreateBookingRequest
{
    public int TourId { get; set; }
    public int TravelAgentId { get; set; }
}
