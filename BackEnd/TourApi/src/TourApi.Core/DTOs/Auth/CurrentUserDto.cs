namespace TourApi.DTOs.Auth;

public class CurrentUserDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = null!;
    public string Role { get; set; } = null!;
}