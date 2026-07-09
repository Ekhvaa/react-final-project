namespace TourApi.Common;

public sealed class EmailOptions
{
    public const string SectionName = "Email";
    public bool RequireConfirmedEmail { get; set; }
    public string FrontendBaseUrl { get; set; } = "http://localhost:5173";
    public int ConfirmationTokenExpiryHours { get; set; } = 24;
    public int PasswordResetTokenExpiryMinutes { get; set; } = 30;
}
