namespace TourApi.Common;

public sealed class FacebookAuthOptions
{
    public const string SectionName = "Authentication:Facebook";

    public string AppId { get; init; } = string.Empty;
    public string AppSecret { get; init; } = string.Empty;
}
