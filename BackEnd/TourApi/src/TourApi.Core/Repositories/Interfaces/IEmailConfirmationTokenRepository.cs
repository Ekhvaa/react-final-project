using TourApi.Models;

namespace TourApi.Repositories;

public interface IEmailConfirmationTokenRepository : IRepository<EmailConfirmationToken>
{
    Task<EmailConfirmationToken?> GetActiveByHashAsync(string tokenHash, CancellationToken cancellationToken = default);
}
