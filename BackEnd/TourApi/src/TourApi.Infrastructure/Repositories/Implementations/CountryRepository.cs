using Microsoft.EntityFrameworkCore;
using TourApi.Data;
using TourApi.Models;

namespace TourApi.Repositories;

public sealed class CountryRepository : Repository<Country>, ICountryRepository
{
    public CountryRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<List<Country>> ListActiveAsync(CancellationToken cancellationToken = default) =>
        DbSet
            .Where(country => !country.IsDeleted)
            .OrderBy(country => country.Name)
            .ToListAsync(cancellationToken);

    public Task<Country?> GetActiveByIdAsync(int id, CancellationToken cancellationToken = default) =>
        DbSet.FirstOrDefaultAsync(country => country.Id == id && !country.IsDeleted, cancellationToken);
}
