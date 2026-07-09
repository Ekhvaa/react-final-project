namespace TourApi.DTOs.Users;

public class TouristDto
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string ContactPhone { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public char Gender { get; set; }
    public string NationalId { get; set; } = null!;
    public bool EmailConfirmed { get; set; }
}
