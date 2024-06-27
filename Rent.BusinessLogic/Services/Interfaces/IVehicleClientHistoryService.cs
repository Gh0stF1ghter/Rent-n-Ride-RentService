using Rent.BusinessLogic.Models;

namespace Rent.BusinessLogic.Services.Interfaces;

public interface IVehicleClientHistoryService
{
    Task<IEnumerable<VehicleClientHistoryModel>> GetRangeAsync(int page, int pageSize, CancellationToken cancellationToken);

    Task<VehicleClientHistoryModel> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<VehicleClientHistoryModel> AddAsync(VehicleClientHistoryModel vchModel, CancellationToken cancellationToken);

    Task<VehicleClientHistoryModel> UpdateAsync(VehicleClientHistoryModel vchModel, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}