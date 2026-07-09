using Microsoft.EntityFrameworkCore;
using TourApi.Data;
using TourApi.Models;

namespace TourApi.Repositories;

public sealed class EmailConfirmationTokenRepository : Repository<EmailConfirmationToken>, IEmailConfirmationTokenRepository
{
    public EmailConfirmationTokenRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public Task<EmailConfirmationToken?> GetActiveByHashAsync(string tokenHash, CancellationToken cancellationToken = default) =>
        DbSet.Include(token => token.User)
            .FirstOrDefaultAsync(token => token.TokenHash == tokenHash && !token.IsDeleted, cancellationToken);
}
