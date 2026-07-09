using Microsoft.EntityFrameworkCore;
using TourApi.Data;
using TourApi.Models;

namespace TourApi.Repositories;

public sealed class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<bool> TravelAgentExistsAsync(int travelAgentId, CancellationToken cancellationToken = default) =>
        DbSet
            .OfType<TravelAgent>()
            .AnyAsync(agent => agent.Id == travelAgentId && !agent.IsDeleted, cancellationToken);

    public Task<bool> TourGuideExistsAsync(int tourGuideId, CancellationToken cancellationToken = default) =>
        DbSet
            .OfType<TourGuide>()
            .AnyAsync(guide => guide.Id == tourGuideId && !guide.IsDeleted, cancellationToken);

    public Task<string?> GetRoleByUserIdAsync(int userId, CancellationToken cancellationToken = default) =>
        DbSet
            .Where(employee => employee.UserId == userId && !employee.IsDeleted)
            .Select(employee => EF.Property<string>(employee, "Discriminator"))
            .FirstOrDefaultAsync(cancellationToken);

    public Task<Employee?> GetActiveByUserIdAsync(int userId, CancellationToken cancellationToken = default) =>
        DbSet
            .Include(employee => employee.User)
            .FirstOrDefaultAsync(employee => employee.UserId == userId && !employee.IsDeleted, cancellationToken);

    public Task<Employee?> GetActiveWithUserByIdAsync(int id, CancellationToken cancellationToken = default) =>
        DbSet
            .Include(employee => employee.User)
            .FirstOrDefaultAsync(employee => employee.Id == id && !employee.IsDeleted, cancellationToken);

    public Task<List<Employee>> ListActiveWithUserAsync(CancellationToken cancellationToken = default) =>
        DbSet
            .Include(employee => employee.User)
            .Where(employee => !employee.IsDeleted)
            .OrderBy(employee => employee.LastName)
            .ThenBy(employee => employee.FirstName)
            .ToListAsync(cancellationToken);
}
