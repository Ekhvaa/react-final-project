namespace TourApi.Models;

public class Booking
{
    public int Id { get; set; }

    public int TourId { get; set; }
    public Tour Tour { get; set; } = null!;

    public int TouristId { get; set; }
    public Tourist Tourist { get; set; } = null!;

    public int TravelAgentId { get; set; }
    public Employee TravelAgent { get; set; } = null!;

    public DateTime DateOfBooking { get; set; }
    public decimal PricePaid { get; set; }
    public BookingStatus Status { get; set; }
}
