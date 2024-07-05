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
        var vehicleConnection = _catalogueServiceConnection + vchModel.VehicleId;
        var userConnection = _userServiceConnection + vchModel.ClientId;

        var vehicle = await GetFromServiceAsModelAsync<VehicleModel>(vehicleConnection, cancellationToken);

        var user = await GetFromServiceAsModelAsync<ClientModel>(userConnection, cancellationToken);

        var vchEntity = await repository.GetByIdAsync(vchModel.Id, cancellationToken);

        var oldDateTime = vchEntity.EndDate;

        var addedRentDays = (vchModel.EndDate - vchEntity.EndDate).TotalDays;

        if (addedRentDays <= 0)
            throw new BadRequestException(ExceptionMessages.NewEndDateLessThanCurrent(vchModel.EndDate, vchEntity.EndDate));

        AssignAsRented(addedRentDays, vehicle, user);

        vchModel.Adapt(vchEntity);

        await repository.UpdateAsync(vchEntity, cancellationToken);

        var vchModelToReturn = vchModel.Adapt<VehicleClientHistoryModel>();

        var response = await PutInServiceAsync(userConnection, user, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            vchModelToReturn.EndDate = oldDateTime;

            await UpdateAsync(vchModelToReturn, cancellationToken);

            await ProcessExceptionAsync(response, _userServiceConnection + user.Id, cancellationToken);
        }

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

    private async Task<T> GetFromServiceAsModelAsync<T>(string connectionString, CancellationToken cancellationToken)
    {
        var response = await _retryPolicy.ExecuteAsync(async () =>
            await _httpClient.GetAsync(connectionString, cancellationToken));

        if (!response.IsSuccessStatusCode)
            await ProcessExceptionAsync(response, connectionString, cancellationToken);

        var result = await response.Content.ReadFromJsonAsync<T>(cancellationToken) ??
            throw new NotFoundException(ExceptionMessages.NotFoundInService(nameof(T), connectionString));

        return result;
    }

    private async Task<HttpResponseMessage> PutInServiceAsync<T>(string? connectionString, T entity, CancellationToken cancellationToken) =>
        await _retryPolicy.ExecuteAsync(async () =>
        {
            var response = await _httpClient.PutAsJsonAsync(connectionString, entity, cancellationToken);
            Console.WriteLine(response.StatusCode);

            return response;
        });

    private async Task<HttpResponseMessage> DeleteFromServiceAsync(string? entityConnectionString, CancellationToken cancellationToken) =>
        await _retryPolicy.ExecuteAsync(async () =>
        {
            var response = await _httpClient.DeleteAsync(entityConnectionString, cancellationToken);
            Console.WriteLine(response.StatusCode);

            return response;
        });

    private static void AssignAsRented(double rentDays, VehicleModel vehicleModel, ClientModel clientModel)
    {
        var totalCost = Convert.ToDecimal(rentDays) * vehicleModel.RentCost;

        if (clientModel.Balance < totalCost)
            throw new BadRequestException(ExceptionMessages.InsufficientFunds(clientModel.Id, clientModel.Balance, totalCost));

        clientModel.Balance -= totalCost;

        vehicleModel.IsRented = true;
        clientModel.IsRenting = true;
    }

    private static async Task ProcessExceptionAsync(HttpResponseMessage response, string? connectionString, CancellationToken cancellationToken)
    {
        var exceptionResponse = await response.Content.ReadAsStringAsync(cancellationToken)
            ?? throw new ServiceException(ExceptionMessages.ServiceError(connectionString));

        throw response.StatusCode switch
        {
            HttpStatusCode.NotFound => new NotFoundException(exceptionResponse),
            HttpStatusCode.BadRequest => new BadRequestException(exceptionResponse),
            _ => new ServiceException(ExceptionMessages.ServiceError(connectionString)),
        };
    }
}