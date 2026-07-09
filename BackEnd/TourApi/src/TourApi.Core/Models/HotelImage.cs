namespace TourApi.Models;

public sealed class HotelImage : BaseEntity
{
    public int HotelId { get; set; }
    public Hotel Hotel { get; set; } = null!;
    public string Url { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long Length { get; set; }
    public bool IsCover { get; set; }
}
