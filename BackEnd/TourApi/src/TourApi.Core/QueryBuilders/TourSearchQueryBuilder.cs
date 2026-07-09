using TourApi.DTOs.Tours;
using TourApi.Models;

namespace TourApi.QueryBuilders;

public sealed class TourSearchQueryBuilder : ITourSearchQueryBuilder
{
    public IQueryable<Tour> Apply(IQueryable<Tour> source, TourSearchQuery query)
    {
        var tours = source.Where(t => !t.IsDeleted);

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            tours = tours.Where(t => t.Name.Contains(keyword) || t.Code.Contains(keyword));
        }

        if (query.MinPrice.HasValue)
            tours = tours.Where(t => t.CurrentPrice >= query.MinPrice.Value);

        if (query.MaxPrice.HasValue)
            tours = tours.Where(t => t.CurrentPrice <= query.MaxPrice.Value);

        if (query.CityId.HasValue)
            tours = tours.Where(t => t.TourDetails.Any(td => td.CityId == query.CityId.Value));

        if (query.CountryId.HasValue)
            tours = tours.Where(t => t.TourDetails.Any(td => td.City.CountryId == query.CountryId.Value));

        if (query.DepartingAfter.HasValue)
            tours = tours.Where(t => t.TourDetails.Any(td => td.EstimatedDepartureDate >= query.DepartingAfter.Value));

        if (query.DepartingBefore.HasValue)
            tours = tours.Where(t => t.TourDetails.Any(td => td.EstimatedDepartureDate <= query.DepartingBefore.Value));

        return tours;
    }
}
