namespace TourApi.DTOs.Users;

public class EmployeeDto
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string ContactPhone { get; set; } = null!;
    public string? Experience { get; set; }
}
