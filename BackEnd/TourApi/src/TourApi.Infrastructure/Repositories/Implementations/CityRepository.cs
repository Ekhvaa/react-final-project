using Microsoft.EntityFrameworkCore;
using TourApi.Data;
using TourApi.Models;

namespace TourApi.Repositories;

public sealed class CityRepository : Repository<City>, ICityRepository
{
    public CityRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public IQueryable<City> QueryWithCountry() =>
        DbSet
            .Include(city => city.Country)
            .Where(city => !city.IsDeleted);

    public Task<List<City>> ListActiveWithCountryAsync(int? countryId = null, CancellationToken cancellationToken = default)
    {
        var query = QueryWithCountry();

        if (countryId.HasValue)
        {
            query = query.Where(city => city.CountryId == countryId.Value);
        }

        return query
            .OrderBy(city => city.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<City?> GetActiveWithCountryByIdAsync(int id, CancellationToken cancellationToken = default) =>
        QueryWithCountry().FirstOrDefaultAsync(city => city.Id == id, cancellationToken);

    public Task<int> CountActiveByIdsAsync(IEnumerable<int> cityIds, CancellationToken cancellationToken = default)
    {
        var cityIdList = cityIds.Distinct().ToList();
        return DbSet.CountAsync(city => cityIdList.Contains(city.Id) && !city.IsDeleted, cancellationToken);
    }
}
