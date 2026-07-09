using Microsoft.EntityFrameworkCore;
using TourApi.Data;
using TourApi.Models;

namespace TourApi.Repositories;

public sealed class HotelImageRepository : Repository<HotelImage>, IHotelImageRepository
{
    public HotelImageRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public Task<List<HotelImage>> ListForHotelAsync(int hotelId, CancellationToken cancellationToken = default) =>
        DbSet.Where(image => image.HotelId == hotelId && !image.IsDeleted)
            .OrderByDescending(image => image.IsCover)
            .ThenBy(image => image.Id)
            .ToListAsync(cancellationToken);
}
