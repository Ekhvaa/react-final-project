using TourApi.Models;

namespace TourApi.Repositories;

public interface ITourImageRepository : IRepository<TourImage>
{
    Task<List<TourImage>> ListForTourAsync(int tourId, CancellationToken cancellationToken = default);
}
