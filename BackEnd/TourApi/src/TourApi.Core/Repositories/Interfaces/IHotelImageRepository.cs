using TourApi.Models;

namespace TourApi.Repositories;

public interface IHotelImageRepository : IRepository<HotelImage>
{
    Task<List<HotelImage>> ListForHotelAsync(int hotelId, CancellationToken cancellationToken = default);
}
