using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TourApi.Data;

namespace TourApi.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly ApplicationDbContext DbContext;
    protected readonly DbSet<TEntity> DbSet;

    public Repository(ApplicationDbContext dbContext)
    {
        DbContext = dbContext;
        DbSet = dbContext.Set<TEntity>();
    }

    public IQueryable<TEntity> Query() => DbSet.AsQueryable();

    public Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        DbSet.FindAsync([id], cancellationToken).AsTask();

    public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) =>
        DbSet.AnyAsync(predicate, cancellationToken);

    public void Add(TEntity entity) => DbSet.Add(entity);

    public void AddRange(IEnumerable<TEntity> entities) => DbSet.AddRange(entities);

    public void Update(TEntity entity) => DbSet.Update(entity);

    public void Remove(TEntity entity) => DbSet.Remove(entity);

    public void RemoveRange(IEnumerable<TEntity> entities) => DbSet.RemoveRange(entities);
}
