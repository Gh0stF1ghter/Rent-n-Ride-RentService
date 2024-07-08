using Microsoft.EntityFrameworkCore;
using Rent.DataAccess.Context;
using Rent.DataAccess.Entities;
using Rent.DataAccess.Repositories.Interfaces;

namespace Rent.DataAccess.Repositories.Implementations;

public class VehicleClientHistoryRepository(AgencyDbContext context) : RepositoryBase<VehicleClientHistoryEntity>(context), IVehicleClientHistoryRepository
{
    public async Task<IEnumerable<VehicleClientHistoryEntity>> GetRangeAsync(int page, int pageSize, CancellationToken cancellationToken) =>
        await GetRange(page, pageSize)
            .ToListAsync(cancellationToken);

    public async Task<VehicleClientHistoryEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        await GetByCondition(vch => vch.Id == id)
            .FirstAsync(cancellationToken);

    public async Task RemoveByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await GetByIdAsync(id, cancellationToken);

        await RemoveAsync(entity, cancellationToken);
    }
}