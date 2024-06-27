using Microsoft.EntityFrameworkCore;
using Rent.DataAccess.Context;
using Rent.DataAccess.Repositories.Interfaces;
using System.Linq.Expressions;

namespace Rent.DataAccess.Repositories.Implementations;

public class RepositoryBase<TEntity>(AgencyDbContext context) : IRepositoryBase<TEntity> where TEntity : class
{
    public IQueryable<TEntity> GetRange(int page, int pageSize)
    {
        var rowsToSkip = (page - 1) * pageSize;

        return context.Set<TEntity>()
            .Skip(rowsToSkip)
            .Take(pageSize)
            .AsNoTracking();
    }

    public IQueryable<TEntity> GetByCondition(Expression<Func<TEntity, bool>> condition) =>
        context.Set<TEntity>()
        .Where(condition)
        .AsNoTracking();

    public Task<bool> IsExistsAsync(Expression<Func<TEntity, bool>> condition, CancellationToken cancellationToken) =>
        context.Set<TEntity>().AnyAsync(condition, cancellationToken);

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        await context.Set<TEntity>().AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(TEntity newEntity, CancellationToken cancellationToken)
    {
        context.Set<TEntity>().Update(newEntity);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(TEntity entityToRemove, CancellationToken cancellationToken)
    {
        context.Set<TEntity>().Remove(entityToRemove);
        await context.SaveChangesAsync(cancellationToken);
    }
}