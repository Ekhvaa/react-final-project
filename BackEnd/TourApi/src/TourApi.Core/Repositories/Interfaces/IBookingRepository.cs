using TourApi.Models;

namespace TourApi.Repositories;

public interface IBookingRepository : IRepository<Booking>
{
    IQueryable<Booking> QueryWithDetails();
    Task<List<Booking>> ListForTouristAsync(int touristId, CancellationToken cancellationToken = default);
    Task<List<Booking>> ListForTravelAgentAsync(int travelAgentId, CancellationToken cancellationToken = default);
    Task<Booking?> GetWithDetailsByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> HasOpenBookingAsync(int touristId, int tourId, CancellationToken cancellationToken = default);
}
