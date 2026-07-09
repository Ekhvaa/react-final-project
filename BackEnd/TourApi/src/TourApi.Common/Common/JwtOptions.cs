namespace TourApi.Common;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Key { get; init; } = string.Empty;
    public string? Issuer { get; init; }
    public string? Audience { get; init; }
    public int ExpiryMinutes { get; init; } = 60;
}
