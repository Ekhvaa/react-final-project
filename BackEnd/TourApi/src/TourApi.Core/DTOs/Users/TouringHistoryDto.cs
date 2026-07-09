namespace TourApi.DTOs.Users;

public class TouringHistoryDto
{
    public int TourId { get; set; }
    public string TourCode { get; set; } = null!;
    public string TourName { get; set; } = null!;
    public DateTime DepartureDate { get; set; }
    public DateTime ReturnDate { get; set; }
}
