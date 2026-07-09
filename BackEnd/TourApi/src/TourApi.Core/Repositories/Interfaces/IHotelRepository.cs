using TourApi.Models;

namespace TourApi.Repositories;

public interface IHotelRepository : IRepository<Hotel>
{
    IQueryable<Hotel> QueryWithDetails();
    Task<List<Hotel>> ListActiveWithDetailsAsync(int? cityId = null, CancellationToken cancellationToken = default);
    Task<Hotel?> GetActiveWithDetailsByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Hotel?> GetActiveForUpdateAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CountActiveByIdsAsync(IEnumerable<int> hotelIds, CancellationToken cancellationToken = default);
}
