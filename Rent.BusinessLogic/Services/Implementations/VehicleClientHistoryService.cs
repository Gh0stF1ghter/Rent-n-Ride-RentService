using Mapster;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Polly;
using Polly.Retry;
using Rent.BusinessLogic.Exceptions;
using Rent.BusinessLogic.Exceptions.ExceptionMessages;
using Rent.BusinessLogic.Extensions;
using Rent.BusinessLogic.Models;
using Rent.BusinessLogic.Services.Interfaces;
using Rent.DataAccess.Entities;
using Rent.DataAccess.Repositories.Interfaces;
using System.Net;
using System.Net.Http.Json;

namespace Rent.BusinessLogic.Services.Implementations;

public class VehicleClientHistoryService(
    IVehicleClientHistoryRepository repository,
    IDistributedCache distributedCache,
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration
    ) : IVehicleClientHistoryService
{
    private readonly string? _userServiceConnection = configuration.GetConnectionString("UserServiceConnection");
    private readonly string? _catalogueServiceConnection = configuration.GetConnectionString("CatalogueServiceConnection");

    private readonly HttpClient _httpClient = httpClientFactory.CreateClient();

    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy = Policy
        .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
        .Or<HttpRequestException>()
        .WaitAndRetryAsync(2, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)), onRetry: (outcome, timespan, retryAttempt, context) =>
        {
            Console.WriteLine($"Error while proccessing request. Error:{outcome.Exception?.Message} Attempt: {retryAttempt} Waiting:{timespan} to retry");
        });

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
        var vehicleConnection = _catalogueServiceConnection + vchModel.VehicleId;
        var userConnection = _userServiceConnection + vchModel.ClientId;

        var totalRentDays = (vchModel.EndDate - vchModel.StartDate).TotalDays;

        var vehicleResponse = await _retryPolicy.ExecuteAsync(async () =>
            await _httpClient.GetAsync(configuration.GetConnectionString("CatalogueServiceConnection") + vchModel.VehicleId, cancellationToken));

        var vehicle = await vehicleResponse.Content.ReadFromJsonAsync<VehicleModel>(cancellationToken);

        var totalCost = Convert.ToDecimal(totalRentDays) * vehicle.RentCost;

        var userResponse = await _retryPolicy.ExecuteAsync(async () =>
            await _httpClient.GetAsync(configuration.GetConnectionString("UserServiceConnection") + vchModel.ClientId, cancellationToken)
            );

        var user = await userResponse.Content.ReadFromJsonAsync<ClientModel>(cancellationToken);

        if (user.Balance < totalCost)
        {

        }

        user.Balance -= totalCost;

        var vch = vchModel.Adapt<VehicleClientHistoryEntity>();

        await repository.AddAsync(vch, cancellationToken);

        await _retryPolicy.ExecuteAsync(async () =>
        {
            var response = await _httpClient.PutAsJsonAsync(configuration.GetConnectionString("UserServiceConnection") + vchModel.ClientId, user, cancellationToken);
            Console.WriteLine(response.StatusCode);

            return response;
        }
        );

        var resultResponse = await _retryPolicy.ExecuteAsync(async () =>
            await _httpClient.GetAsync(configuration.GetConnectionString("UserServiceConnection") + vchModel.ClientId, cancellationToken)
            );

        var result = await resultResponse.Content.ReadFromJsonAsync<ClientModel>(cancellationToken);

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