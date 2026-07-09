using Microsoft.EntityFrameworkCore;
using TourApi.Data;
using TourApi.Models;

namespace TourApi.Repositories;

public sealed class BookingRepository : Repository<Booking>, IBookingRepository
{
    public BookingRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public IQueryable<Booking> QueryWithDetails() =>
        DbSet
            .Include(booking => booking.Tour)
            .Include(booking => booking.Tourist)
            .Include(booking => booking.TravelAgent);

    public Task<List<Booking>> ListForTouristAsync(int touristId, CancellationToken cancellationToken = default) =>
        QueryWithDetails()
            .Where(booking => booking.TouristId == touristId)
            .OrderByDescending(booking => booking.DateOfBooking)
            .ToListAsync(cancellationToken);

    public Task<List<Booking>> ListForTravelAgentAsync(int travelAgentId, CancellationToken cancellationToken = default) =>
        QueryWithDetails()
            .Where(booking => booking.TravelAgentId == travelAgentId)
            .OrderByDescending(booking => booking.DateOfBooking)
            .ToListAsync(cancellationToken);

    public Task<Booking?> GetWithDetailsByIdAsync(int id, CancellationToken cancellationToken = default) =>
        QueryWithDetails().FirstOrDefaultAsync(booking => booking.Id == id, cancellationToken);

    public Task<bool> HasOpenBookingAsync(int touristId, int tourId, CancellationToken cancellationToken = default) =>
        DbSet.AnyAsync(booking => booking.TouristId == touristId
            && booking.TourId == tourId
            && booking.Status != BookingStatus.Cancelled,
            cancellationToken);
}
