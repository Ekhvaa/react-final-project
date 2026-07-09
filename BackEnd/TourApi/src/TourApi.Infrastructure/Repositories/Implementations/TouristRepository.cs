using Microsoft.EntityFrameworkCore;
using TourApi.Data;
using TourApi.Models;

namespace TourApi.Repositories;

public sealed class TouristRepository : Repository<Tourist>, ITouristRepository
{
    public TouristRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<Tourist?> GetActiveByUserIdAsync(int userId, CancellationToken cancellationToken = default) =>
        DbSet.FirstOrDefaultAsync(tourist => tourist.UserId == userId && !tourist.IsDeleted, cancellationToken);

    public Task<Tourist?> GetActiveByUserIdWithUserAsync(int userId, CancellationToken cancellationToken = default) =>
        DbSet
            .Include(tourist => tourist.User)
            .FirstOrDefaultAsync(tourist => tourist.UserId == userId && !tourist.IsDeleted, cancellationToken);
}
