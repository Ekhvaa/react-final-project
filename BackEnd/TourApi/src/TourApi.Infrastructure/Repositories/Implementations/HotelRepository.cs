using Microsoft.EntityFrameworkCore;
using TourApi.Data;
using TourApi.Models;

namespace TourApi.Repositories;

public sealed class HotelRepository : Repository<Hotel>, IHotelRepository
{
    public HotelRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public IQueryable<Hotel> QueryWithDetails() =>
        DbSet
            .Include(hotel => hotel.City)
                .ThenInclude(city => city.Country)
            .Include(hotel => hotel.HotelServiceMappings)
                .ThenInclude(mapping => mapping.HotelService)
            .Include(hotel => hotel.Images)
            .Where(hotel => !hotel.IsDeleted);

    public Task<List<Hotel>> ListActiveWithDetailsAsync(int? cityId = null, CancellationToken cancellationToken = default)
    {
        var query = QueryWithDetails();

        if (cityId.HasValue)
        {
            query = query.Where(hotel => hotel.CityId == cityId.Value);
        }

        return query
            .OrderBy(hotel => hotel.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<Hotel?> GetActiveWithDetailsByIdAsync(int id, CancellationToken cancellationToken = default) =>
        QueryWithDetails().FirstOrDefaultAsync(hotel => hotel.Id == id, cancellationToken);

    public Task<Hotel?> GetActiveForUpdateAsync(int id, CancellationToken cancellationToken = default) =>
        DbSet
            .Include(hotel => hotel.HotelServiceMappings)
            .FirstOrDefaultAsync(hotel => hotel.Id == id && !hotel.IsDeleted, cancellationToken);

    public Task<int> CountActiveByIdsAsync(IEnumerable<int> hotelIds, CancellationToken cancellationToken = default)
    {
        var hotelIdList = hotelIds.Distinct().ToList();
        return DbSet.CountAsync(hotel => hotelIdList.Contains(hotel.Id) && !hotel.IsDeleted, cancellationToken);
    }
}
