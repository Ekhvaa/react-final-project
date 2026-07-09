using TourApi.Models;

namespace TourApi.Repositories;

public interface ICityRepository : IRepository<City>
{
    IQueryable<City> QueryWithCountry();
    Task<List<City>> ListActiveWithCountryAsync(int? countryId = null, CancellationToken cancellationToken = default);
    Task<City?> GetActiveWithCountryByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CountActiveByIdsAsync(IEnumerable<int> cityIds, CancellationToken cancellationToken = default);
}
