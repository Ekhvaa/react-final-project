using TourApi.DTOs.Tours;
using TourApi.Models;
using TourApi.QueryBuilders;

namespace TourApi.Repositories;

public interface ITourRepository : IRepository<Tour>
{
    IQueryable<Tour> QueryWithDetails();
    Task<Tour?> GetActiveByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Tour?> GetActiveWithDetailsByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Tour?> GetActiveForUpdateAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Tour>> ListAssignedToTourGuideAsync(int tourGuideId, CancellationToken cancellationToken = default);
    Task<bool> IsCodeTakenAsync(string code, int? currentTourId = null, CancellationToken cancellationToken = default);
    Task<(List<Tour> Items, int TotalCount)> SearchAsync(
        TourSearchQuery query,
        ITourSearchQueryBuilder searchQueryBuilder,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
