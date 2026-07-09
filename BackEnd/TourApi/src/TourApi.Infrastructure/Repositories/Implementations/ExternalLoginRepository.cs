using Microsoft.EntityFrameworkCore;
using TourApi.Data;
using TourApi.Models;

namespace TourApi.Repositories;

public sealed class ExternalLoginRepository : Repository<ExternalLogin>, IExternalLoginRepository
{
    public ExternalLoginRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<ExternalLogin?> GetByProviderAsync(
        string provider,
        string providerUserId,
        CancellationToken cancellationToken = default)
    {
        return DbSet
            .Include(login => login.User)
            .FirstOrDefaultAsync(
                login => login.Provider == provider
                    && login.ProviderUserId == providerUserId
                    && !login.IsDeleted
                    && !login.User.IsDeleted,
                cancellationToken);
    }
}
