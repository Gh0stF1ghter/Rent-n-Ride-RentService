using Rent.DataAccess.Entities;

namespace Rent.DataAccess.Repositories.Interfaces;

public interface IVehicleClientHistoryRepository : IRepositoryBase<VehicleClientHistoryEntity>
{
    Task<IEnumerable<VehicleClientHistoryEntity>> GetRangeAsync(int page, int pageSize, CancellationToken cancellationToken);

    Task<VehicleClientHistoryEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}