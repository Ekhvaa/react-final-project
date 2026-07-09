namespace TourApi.Models;

public class Tourist : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public string ContactPhone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public string FirstName { get; set; } = null!;

    // 'F' or 'M'
    public char Gender { get; set; }

    public string LastName { get; set; } = null!;
    public string NationalId { get; set; } = null!;

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<TouringHistory> TouringHistories { get; set; } = new List<TouringHistory>();
}
