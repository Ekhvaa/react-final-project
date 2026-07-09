namespace TourApi.DTOs.Images;

public sealed class ImageDto
{
    public int Id { get; set; }
    public string Url { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long Length { get; set; }
    public bool IsCover { get; set; }
}
