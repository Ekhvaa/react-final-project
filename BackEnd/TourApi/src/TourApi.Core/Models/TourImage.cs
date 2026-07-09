namespace TourApi.Models;

public sealed class TourImage : BaseEntity
{
    public int TourId { get; set; }
    public Tour Tour { get; set; } = null!;
    public string Url { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long Length { get; set; }
    public bool IsCover { get; set; }
}
