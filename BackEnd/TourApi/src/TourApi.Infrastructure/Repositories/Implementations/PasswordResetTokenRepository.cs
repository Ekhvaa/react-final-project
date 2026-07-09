using Microsoft.EntityFrameworkCore;
using TourApi.Data;
using TourApi.Models;

namespace TourApi.Repositories;

public sealed class PasswordResetTokenRepository : Repository<PasswordResetToken>, IPasswordResetTokenRepository
{
    public PasswordResetTokenRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public Task<PasswordResetToken?> GetActiveByHashAsync(string tokenHash, CancellationToken cancellationToken = default) =>
        DbSet.Include(token => token.User)
            .FirstOrDefaultAsync(token => token.TokenHash == tokenHash && !token.IsDeleted, cancellationToken);
}
