namespace TourApi.Common;

public sealed class RefreshTokenOptions
{
    public const string SectionName = "RefreshTokens";
    public int ExpiryDays { get; set; } = 14;
}
