using TourApi.Models;

namespace TourApi.Repositories;

public interface ICountryRepository : IRepository<Country>
{
    Task<List<Country>> ListActiveAsync(CancellationToken cancellationToken = default);
    Task<Country?> GetActiveByIdAsync(int id, CancellationToken cancellationToken = default);
}
