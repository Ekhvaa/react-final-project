using Microsoft.EntityFrameworkCore;
using TourApi.Data;
using TourApi.Models;

namespace TourApi.Repositories;

public sealed class TouringHistoryRepository : Repository<TouringHistory>, ITouringHistoryRepository
{
    public TouringHistoryRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<List<TouringHistory>> ListForTouristAsync(int touristId, CancellationToken cancellationToken = default) =>
        DbSet
            .Include(history => history.Tour)
            .Where(history => history.TouristId == touristId)
            .OrderByDescending(history => history.DepartureDate)
            .ToListAsync(cancellationToken);
}
