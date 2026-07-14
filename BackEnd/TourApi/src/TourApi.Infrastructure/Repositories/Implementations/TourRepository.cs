using Microsoft.EntityFrameworkCore;
using TourApi.Data;
using TourApi.DTOs.Tours;
using TourApi.Models;
using TourApi.QueryBuilders;

namespace TourApi.Repositories;

public sealed class TourRepository : Repository<Tour>, ITourRepository
{
    public TourRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public IQueryable<Tour> QueryWithDetails() =>
        DbSet
            .Include(tour => tour.TourDetails)
                .ThenInclude(detail => detail.City)
                    .ThenInclude(city => city.Country)
            .Include(tour => tour.TourDetails)
                .ThenInclude(detail => detail.Hotel)
            .Include(tour => tour.Images)
            .Include(tour => tour.CreatedByEmployee)
            .Include(tour => tour.AssignedTourGuide)
            .Include(tour => tour.AssignedTravelAgent);

    public Task<Tour?> GetActiveByIdAsync(int id, CancellationToken cancellationToken = default) =>
        DbSet.FirstOrDefaultAsync(tour => tour.Id == id && !tour.IsDeleted, cancellationToken);

    public Task<Tour?> GetActiveWithDetailsByIdAsync(int id, CancellationToken cancellationToken = default) =>
        QueryWithDetails().FirstOrDefaultAsync(tour => tour.Id == id && !tour.IsDeleted, cancellationToken);

    public Task<Tour?> GetActiveForUpdateAsync(int id, CancellationToken cancellationToken = default) =>
        DbSet
            .Include(tour => tour.TourDetails)
            .FirstOrDefaultAsync(tour => tour.Id == id && !tour.IsDeleted, cancellationToken);

    public Task<List<Tour>> ListAssignedToTourGuideAsync(int tourGuideId, CancellationToken cancellationToken = default) =>
        QueryWithDetails()
            .Where(tour => !tour.IsDeleted && tour.AssignedTourGuideId == tourGuideId)
            .OrderBy(tour => tour.Name)
            .ToListAsync(cancellationToken);

    public Task<bool> IsCodeTakenAsync(string code, int? currentTourId = null, CancellationToken cancellationToken = default) =>
        DbSet.AnyAsync(
            tour => tour.Code == code && (!currentTourId.HasValue || tour.Id != currentTourId.Value),
            cancellationToken);
    public async Task<(List<Tour> Items, int TotalCount)> SearchAsync(
        TourSearchQuery query,
        ITourSearchQueryBuilder searchQueryBuilder,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var toursQuery = searchQueryBuilder.Apply(QueryWithDetails(), query);

        var totalCount = await toursQuery.CountAsync(cancellationToken);

        toursQuery = ApplySorting(toursQuery, query);

        var tours = await toursQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (tours, totalCount);
    }

    private static IQueryable<Tour> ApplySorting(IQueryable<Tour> queryable, TourSearchQuery query)
    {
        var descending = string.Equals(query.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);
        return query.SortBy?.ToLowerInvariant() switch
        {
            "price" => descending ? queryable.OrderByDescending(tour => tour.CurrentPrice) : queryable.OrderBy(tour => tour.CurrentPrice),
            "date" => descending
                ? queryable.OrderByDescending(tour => tour.TourDetails.Min(detail => detail.EstimatedArrivalDate))
                : queryable.OrderBy(tour => tour.TourDetails.Min(detail => detail.EstimatedArrivalDate)),
            "name" => descending ? queryable.OrderByDescending(tour => tour.Name) : queryable.OrderBy(tour => tour.Name),
            _ => queryable.OrderBy(tour => tour.Name)
        };
    }
}
