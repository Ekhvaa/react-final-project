namespace TourApi.Models;

// Discriminator column in the DB (nvarchar(13)) implies table-per-hierarchy.
// Adjust/rename the derived types below to match whatever employee roles
// your app actually needs (e.g. Admin, TourGuide, TravelAgent...).
public abstract class Employee : BaseEntity
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
    public string? Experience { get; set; }

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
