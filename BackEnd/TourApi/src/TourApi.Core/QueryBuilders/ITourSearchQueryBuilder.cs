using TourApi.DTOs.Tours;
using TourApi.Models;

namespace TourApi.QueryBuilders;

public interface ITourSearchQueryBuilder
{
    IQueryable<Tour> Apply(IQueryable<Tour> source, TourSearchQuery query);
}
