using TourApi.Models;

namespace TourApi.Repositories;

public interface IHotelServiceRepository : IRepository<HotelService>
{
    Task<List<HotelService>> ListOrderedAsync(CancellationToken cancellationToken = default);
    Task<int> CountExistingByIdsAsync(IEnumerable<int> serviceIds, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
}
