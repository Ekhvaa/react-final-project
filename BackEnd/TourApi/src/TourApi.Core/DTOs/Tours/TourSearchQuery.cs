namespace TourApi.DTOs.Tours;

public class TourSearchQuery
{
    public string? Keyword { get; set; }
    public int? CityId { get; set; }
    public int? CountryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public DateTime? DepartingAfter { get; set; }
    public DateTime? DepartingBefore { get; set; }
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
