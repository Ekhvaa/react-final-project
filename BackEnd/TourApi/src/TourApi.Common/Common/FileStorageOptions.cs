namespace TourApi.Common;

public sealed class FileStorageOptions
{
    public const string SectionName = "FileStorage";
    public string RootPath { get; set; } = "wwwroot/uploads";
    public string PublicBasePath { get; set; } = "/uploads";
    public long MaxImageBytes { get; set; } = 5 * 1024 * 1024;
}
