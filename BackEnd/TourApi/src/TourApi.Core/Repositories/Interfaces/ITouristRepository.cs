using TourApi.Models;

namespace TourApi.Repositories;

public interface ITouristRepository : IRepository<Tourist>
{
    Task<Tourist?> GetActiveByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<Tourist?> GetActiveByUserIdWithUserAsync(int userId, CancellationToken cancellationToken = default);
}
