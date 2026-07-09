using TourApi.Models;

namespace TourApi.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<bool> IsUsernameTakenAsync(string username, CancellationToken cancellationToken = default);
    Task<User?> GetActiveByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<User?> GetActiveByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> AnyAdminExistsAsync(CancellationToken cancellationToken = default);
}
