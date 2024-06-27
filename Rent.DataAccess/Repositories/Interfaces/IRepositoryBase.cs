using System.Linq.Expressions;

namespace Rent.DataAccess.Repositories.Interfaces;

public interface IRepositoryBase<TEntity> where TEntity : class
{
    IQueryable<TEntity> GetRange(int page, int pageSize);

    IQueryable<TEntity> GetByCondition(Expression<Func<TEntity, bool>> condition);

    Task<bool> IsExistsAsync(Expression<Func<TEntity, bool>> condition, CancellationToken cancellationToken);

    Task AddAsync(TEntity entity, CancellationToken cancellationToken);

    Task UpdateAsync(TEntity newEntity, CancellationToken cancellationToken);

    Task RemoveAsync(TEntity entityToRemove, CancellationToken cancellationToken);
}