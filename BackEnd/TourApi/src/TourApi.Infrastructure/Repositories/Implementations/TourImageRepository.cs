using Microsoft.EntityFrameworkCore;
using TourApi.Data;
using TourApi.Models;

namespace TourApi.Repositories;

public sealed class TourImageRepository : Repository<TourImage>, ITourImageRepository
{
    public TourImageRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public Task<List<TourImage>> ListForTourAsync(int tourId, CancellationToken cancellationToken = default) =>
        DbSet.Where(image => image.TourId == tourId && !image.IsDeleted)
            .OrderByDescending(image => image.IsCover)
            .ThenBy(image => image.Id)
            .ToListAsync(cancellationToken);
}
