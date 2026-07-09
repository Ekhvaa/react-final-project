using TourApi.Models;

namespace TourApi.Repositories;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<bool> TravelAgentExistsAsync(int travelAgentId, CancellationToken cancellationToken = default);
    Task<bool> TourGuideExistsAsync(int tourGuideId, CancellationToken cancellationToken = default);
    Task<string?> GetRoleByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<Employee?> GetActiveByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<Employee?> GetActiveWithUserByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Employee>> ListActiveWithUserAsync(CancellationToken cancellationToken = default);
}
