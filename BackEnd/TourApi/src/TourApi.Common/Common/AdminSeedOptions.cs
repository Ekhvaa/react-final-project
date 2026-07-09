namespace TourApi.Common;

public sealed class AdminSeedOptions
{
    public const string SectionName = "SeedAdmin";
    public bool Enabled { get; set; }
    public string Username { get; set; } = "admin";
    public string Password { get; set; } = "Admin123!";
    public string FirstName { get; set; } = "System";
    public string LastName { get; set; } = "Admin";
    public string Email { get; set; } = "admin@tourapi.local";
    public string ContactPhone { get; set; } = "+995000000000";
    public string NationalId { get; set; } = "ADMIN-SEED";
}
