using TourApi.Models;

namespace TourApi.DTOs.Bookings;

public class BookingDto
{
    public int Id { get; set; }
    public int TourId { get; set; }
    public string TourCode { get; set; } = null!;
    public string TourName { get; set; } = null!;
    public int TouristId { get; set; }
    public string TouristFullName { get; set; } = null!;
    public int TravelAgentId { get; set; }
    public string TravelAgentFullName { get; set; } = null!;
    public DateTime DateOfBooking { get; set; }
    public decimal PricePaid { get; set; }
    public BookingStatus Status { get; set; }
}
