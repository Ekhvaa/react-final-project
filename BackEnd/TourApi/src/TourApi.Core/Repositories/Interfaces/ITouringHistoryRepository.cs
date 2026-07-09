using TourApi.Models;

namespace TourApi.Repositories;

public interface ITouringHistoryRepository : IRepository<TouringHistory>
{
    Task<List<TouringHistory>> ListForTouristAsync(int touristId, CancellationToken cancellationToken = default);
}
