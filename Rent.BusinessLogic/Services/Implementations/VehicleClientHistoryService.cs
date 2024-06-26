using Mapster;
using Microsoft.Extensions.Caching.Distributed;
using Rent.BusinessLogic.Extensions;
using Rent.BusinessLogic.Models;
using Rent.BusinessLogic.Services.Interfaces;
using Rent.DataAccess.Entities;
using Rent.DataAccess.Repositories.Interfaces;

namespace Rent.BusinessLogic.Services.Implementations;

public class VehicleClientHistoryService(IVehicleClientHistoryRepository repository, IDistributedCache distributedCache) : IVehicleClientHistoryService
{
    public async Task<IEnumerable<VehicleClientHistoryModel>> GetRangeAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        var vehicleClientHistories = await repository.GetRangeAsync(page, pageSize, cancellationToken);

        var vehicleClientHistoryModels = vehicleClientHistories.Adapt<IEnumerable<VehicleClientHistoryModel>>();

        return vehicleClientHistoryModels;
    }

    public async Task<VehicleClientHistoryModel> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var key = nameof(VehicleClientHistoryModel) + id;

        var cache = await distributedCache.GetDataFromCacheAsync<VehicleClientHistoryModel>(key, cancellationToken);

        if (cache is not null)
            return cache;

        var vch = await repository.GetByIdAsync(id, cancellationToken);

        var vchModel = vch.Adapt<VehicleClientHistoryModel>();

        await distributedCache.CacheData(vchModel, key, cancellationToken);

        return vchModel;
    }

    public async Task<VehicleClientHistoryModel> AddAsync(VehicleClientHistoryModel vchModel, CancellationToken cancellationToken)
    {
        var vch = vchModel.Adapt<VehicleClientHistoryEntity>();

        await repository.AddAsync(vch, cancellationToken);

        var newVchModel = vch.Adapt<VehicleClientHistoryModel>();

        return newVchModel;
    }

    public async Task<VehicleClientHistoryModel> UpdateAsync(VehicleClientHistoryModel vchModel, CancellationToken cancellationToken)
    {
        var newVchModel = await repository.GetByIdAsync(vchModel.Id, cancellationToken);

        vchModel.Adapt(newVchModel);

        await repository.UpdateAsync(newVchModel, cancellationToken);

        var vchModelToReturn = vchModel.Adapt<VehicleClientHistoryModel>();

        var key = nameof(VehicleClientHistoryModel) + vchModelToReturn.Id;

        await distributedCache.CacheData(vchModelToReturn, key, cancellationToken);

        return vchModelToReturn;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var modelName = await repository.GetByIdAsync(id, cancellationToken);

        await repository.RemoveAsync(modelName, cancellationToken);

        var key = nameof(VehicleClientHistoryModel) + id;
        await distributedCache.RemoveAsync(key, cancellationToken);
    }
}