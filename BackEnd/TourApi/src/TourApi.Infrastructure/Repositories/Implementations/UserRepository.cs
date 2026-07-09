using Microsoft.EntityFrameworkCore;
using TourApi.Data;
using TourApi.Models;

namespace TourApi.Repositories;

public sealed class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<bool> IsUsernameTakenAsync(string username, CancellationToken cancellationToken = default) =>
        DbSet.AnyAsync(user => user.Username == username && !user.IsDeleted, cancellationToken);

    public Task<User?> GetActiveByUsernameAsync(string username, CancellationToken cancellationToken = default) =>
        DbSet.FirstOrDefaultAsync(user => user.Username == username && !user.IsDeleted, cancellationToken);

    public Task<User?> GetActiveByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return DbSet
            .Include(user => user.Tourist)
            .Include(user => user.Employee)
            .FirstOrDefaultAsync(
                user => !user.IsDeleted
                    && ((user.Tourist != null && !user.Tourist.IsDeleted && user.Tourist.Email == email)
                        || (user.Employee != null && !user.Employee.IsDeleted && user.Employee.Email == email)),
                cancellationToken);
    }

    public Task<bool> AnyAdminExistsAsync(CancellationToken cancellationToken = default) =>
        DbSet.Include(user => user.Employee)
            .AnyAsync(user => !user.IsDeleted
                && user.Employee != null
                && !user.Employee.IsDeleted
                && EF.Property<string>(user.Employee, "Discriminator") == nameof(Admin), cancellationToken);
}
