using Microsoft.EntityFrameworkCore;
using TourApi.Data;
using TourApi.Models;

namespace TourApi.Repositories;

public sealed class HotelServiceRepository : Repository<HotelService>, IHotelServiceRepository
{
    public HotelServiceRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<List<HotelService>> ListOrderedAsync(CancellationToken cancellationToken = default) =>
        DbSet
            .OrderBy(service => service.Name)
            .ToListAsync(cancellationToken);

    public Task<int> CountExistingByIdsAsync(IEnumerable<int> serviceIds, CancellationToken cancellationToken = default)
    {
        var serviceIdList = serviceIds.Distinct().ToList();
        return DbSet.CountAsync(service => serviceIdList.Contains(service.Id), cancellationToken);
    }
}
