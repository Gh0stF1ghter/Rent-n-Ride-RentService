using FluentAssertions;
using Mapster;
using Moq;
using Newtonsoft.Json;
using Rent.BusinessLogic.Exceptions;
using Rent.BusinessLogic.Models;
using Rent.BusinessLogic.Services.Implementations;
using Rent.DataAccess.Entities;
using Rent.UnitTests.DataGeneration;
using Rent.UnitTests.Mocks;
using SoloX.CodeQuality.Test.Helpers.Http;
using System.Net;
using System.Text;

namespace Rent.UnitTests.ServicesTests;

public class VehicleClientHistoryServiceTests
{
    private readonly ConfigurationMock _configurationMock = new();
    private readonly HttpClientFactoryMock _httpClientFactoryMock = new();

    private readonly VehicleClientHistoryRepositoryMock _repositoryMock = new();

    private readonly DistributedCacheMock _distributedCacheMock = new();

    private readonly List<VehicleClientHistoryEntity> _vehicleClientHistories = DataGenerator.GenerateVehicleClientHistoryData(5);

    public VehicleClientHistoryServiceTests()
    {
        _configurationMock.GetCatalogueCoonnection();
        _configurationMock.GetUserConnection();

        _httpClientFactoryMock.CreateClient(It.IsAny<HttpClient>());

        _repositoryMock.GetRange(_vehicleClientHistories);
        _repositoryMock.GetById(_vehicleClientHistories[0]);
        _repositoryMock.IsExists(true);
    }

    [Fact]
    public async Task GetRangeAsync__ReturnsVehicleClientHistoryModelList()
    {
        //Arrange
        var correctModels = _vehicleClientHistories.Adapt<IEnumerable<VehicleClientHistoryModel>>();

        var service = new VehicleClientHistoryService(_repositoryMock.Object,
            _distributedCacheMock.Object,
            _httpClientFactoryMock.Object,
            _configurationMock.Object
            );

        //Act
        var response = await service.GetRangeAsync(1, 1, default);

        //Assert
        response.Should().BeEquivalentTo(correctModels);
    }

    [Fact]
    public async Task GetByIdAsync__ReturnsVehicleClientHistoryModel()
    {
        //Arrange
        var correctModel = _vehicleClientHistories[0].Adapt<VehicleClientHistoryModel>();

        var serializedModel = JsonConvert.SerializeObject(correctModel);
        var cachedModel = Encoding.UTF8.GetBytes(serializedModel);
        _distributedCacheMock.GetDataFromCache(cachedModel);

        var service = new VehicleClientHistoryService(_repositoryMock.Object,
            _distributedCacheMock.Object,
            _httpClientFactoryMock.Object,
            _configurationMock.Object
            );

        //Act
        var response = await service.GetByIdAsync(Guid.NewGuid(), default);

        //Assert
        response.Should().BeEquivalentTo(correctModel);
    }

    [Fact]
    public async Task GetByIdAsync_InvalidId_ThrowsInvalidOperationException()
    {
        //Arrange
        _repositoryMock.GetByIdThrowsException();

        var service = new VehicleClientHistoryService(_repositoryMock.Object,
            _distributedCacheMock.Object,
            _httpClientFactoryMock.Object,
            _configurationMock.Object
            );

        //Act
        var response = async () => await service.GetByIdAsync(Guid.NewGuid(), default);

        //Assert
        await response.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task GetByIdAsync_EmptyCache_ReturnsClientModel()
    {
        //Arrange
        var correctModel = _vehicleClientHistories[0].Adapt<VehicleClientHistoryModel>();

        var serializedModel = JsonConvert.SerializeObject(null);
        var cachedModel = Encoding.UTF8.GetBytes(serializedModel);
        _distributedCacheMock.GetDataFromCache(cachedModel);

        var service = new VehicleClientHistoryService(_repositoryMock.Object,
            _distributedCacheMock.Object,
            _httpClientFactoryMock.Object,
            _configurationMock.Object
            );

        //Act
        var response = await service.GetByIdAsync(Guid.NewGuid(), default);

        //Assert
        response.Should().BeEquivalentTo(correctModel);
    }

    [Theory]
    [InlineData(9000, 1000)]
    [InlineData(9000, 9000)]
    public async Task AddAsync_VehicleClientHistoryModel_ReturnsVehicleClientHistoryModel(decimal userBaseBalance, decimal rentCost)
    {
        //Arrange
        var correctModel = _vehicleClientHistories[0].Adapt<VehicleClientHistoryModel>();

        var totalRentDays = (correctModel.EndDate - correctModel.StartDate).TotalDays;

        var clientBalance = userBaseBalance * Convert.ToDecimal(totalRentDays);

        var client = DataGenerator.GenerateClientModel(clientBalance);
        var vehicle = DataGenerator.GenerateVehicleModel(rentCost);

        var httpClient = new HttpClientMockBuilder()
            .BuildHttpClient(client, vehicle);

        _httpClientFactoryMock
            .CreateClient(httpClient);

        var service = new VehicleClientHistoryService(_repositoryMock.Object,
            _distributedCacheMock.Object,
            _httpClientFactoryMock.Object,
            _configurationMock.Object
            );

        //Act
        var response = await service.AddAsync(correctModel, default);

        //Assert
        response.Should().BeEquivalentTo(correctModel);
    }

    [Theory]
    [InlineData(9000, 1000, true, false)]
    [InlineData(9000, 1000, false, true)]
    [InlineData(1000, 9000, false, false)]
    public async Task AddAsync_InvalidServiceRequest_ThrowsBadRequestException(
        decimal userBaseBalance,
        decimal rentCost,
        bool isUserRenting,
        bool isVehicleRented
        )
    {
        //Arrange
        var correctModel = _vehicleClientHistories[0].Adapt<VehicleClientHistoryModel>();

        var totalRentDays = (correctModel.EndDate - correctModel.StartDate).TotalDays;

        var clientBalance = userBaseBalance * Convert.ToDecimal(totalRentDays);

        var client = DataGenerator.GenerateClientModel(clientBalance, isUserRenting);
        var vehicle = DataGenerator.GenerateVehicleModel(rentCost, isVehicleRented);

        var httpClient = new HttpClientMockBuilder()
            .BuildHttpClient(client, vehicle);

        _httpClientFactoryMock
            .CreateClient(httpClient);

        var service = new VehicleClientHistoryService(_repositoryMock.Object,
            _distributedCacheMock.Object,
            _httpClientFactoryMock.Object,
            _configurationMock.Object
            );

        //Act
        var response = async () => await service.AddAsync(correctModel, default);

        //Assert
        await response.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task AddAsync_EndDateTimeGreaterThanStartDateTime_ThrowsBadRequestException()
    {
        //Arrange
        var correctModel = _vehicleClientHistories[0].Adapt<VehicleClientHistoryModel>();

        correctModel.StartDate = DateTime.UtcNow;
        correctModel.EndDate = DateTime.UtcNow - TimeSpan.FromDays(5);

        var service = new VehicleClientHistoryService(_repositoryMock.Object,
            _distributedCacheMock.Object,
            _httpClientFactoryMock.Object,
            _configurationMock.Object
            );

        //Act
        var response = async () => await service.AddAsync(correctModel, default);

        //Assert
        await response.Should().ThrowAsync<BadRequestException>();
    }

    [Theory]
    [InlineData(HttpStatusCode.OK, HttpStatusCode.NotFound, HttpStatusCode.OK, HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.NotFound, HttpStatusCode.OK, HttpStatusCode.OK, HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.OK, HttpStatusCode.OK, HttpStatusCode.NotFound, HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.OK, HttpStatusCode.OK, HttpStatusCode.OK, HttpStatusCode.NotFound)]
    public async Task AddAsync_ServiceResponseIsNullOrEmpty_ThrowsNotFoundException(
        HttpStatusCode clientGetResponseStatusCode,
        HttpStatusCode vehicleGetResponseStatusCode,
        HttpStatusCode clientPutResponseStatusCode,
        HttpStatusCode vehiclePutResponseStatusCode
        )
    {
        //Arrange
        var correctModel = _vehicleClientHistories[0].Adapt<VehicleClientHistoryModel>();

        var client = DataGenerator.GenerateClientModel();
        var vehicle = DataGenerator.GenerateVehicleModel();

        var httpClient = new HttpClientMockBuilder()
            .BuildHttpClient(
            client,
            vehicle,
            clientGetResponseStatus: clientGetResponseStatusCode,
            vehicleGetResponseStatus: vehicleGetResponseStatusCode,
            clientPutResponseStatus: clientPutResponseStatusCode,
            vehiclePutResponseStatus: vehiclePutResponseStatusCode
            );

        _httpClientFactoryMock
            .CreateClient(httpClient);

        var service = new VehicleClientHistoryService(_repositoryMock.Object,
            _distributedCacheMock.Object,
            _httpClientFactoryMock.Object,
            _configurationMock.Object
            );

        //Act
        var response = async () => await service.AddAsync(correctModel, default);

        //Assert
        await response.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task AddAsync_NullClientModel_ThrowsNotFoundException()
    {
        //Arrange
        var correctModel = _vehicleClientHistories[0].Adapt<VehicleClientHistoryModel>();

        var vehicle = DataGenerator.GenerateVehicleModel();

        var httpClient = new HttpClientMockBuilder()
            .BuildHttpClient(default, vehicle);

        _httpClientFactoryMock
            .CreateClient(httpClient);

        var service = new VehicleClientHistoryService(_repositoryMock.Object,
            _distributedCacheMock.Object,
            _httpClientFactoryMock.Object,
            _configurationMock.Object
            );

        //Act
        var response = async () => await service.AddAsync(correctModel, default);

        //Assert
        await response.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task AddAsync_NullVehicleModel_ThrowsNotFoundException()
    {
        //Arrange
        var correctModel = _vehicleClientHistories[0].Adapt<VehicleClientHistoryModel>();

        var client = DataGenerator.GenerateClientModel();

        var httpClient = new HttpClientMockBuilder()
            .BuildHttpClient(client, default);

        _httpClientFactoryMock
            .CreateClient(httpClient);

        var service = new VehicleClientHistoryService(_repositoryMock.Object,
            _distributedCacheMock.Object,
            _httpClientFactoryMock.Object,
            _configurationMock.Object
            );

        //Act
        var response = async () => await service.AddAsync(correctModel, default);

        //Assert
        await response.Should().ThrowAsync<NotFoundException>();
    }

    [Theory]
    [InlineData(9000, 1000)]
    [InlineData(9000, 9000)]
    public async Task UpdateAsync_VehicleClientHistoryModel_ReturnsVehicleClientHistoryModel(decimal userBaseBalance, decimal rentCost)
    {
        //Arrange
        var vehicleClientHistory = DataGenerator.GenerateVehicleClientHistoryData(1)[0];
        vehicleClientHistory.EndDate = DateTime.UtcNow;

        var correctUpdatedModel = vehicleClientHistory.Adapt<VehicleClientHistoryModel>();

        correctUpdatedModel.EndDate = DateTime.UtcNow + TimeSpan.FromDays(4);

        var totalRentDays = (correctUpdatedModel.EndDate - correctUpdatedModel.StartDate).TotalDays;

        var clientBalance = userBaseBalance * Convert.ToDecimal(totalRentDays);

        var client = DataGenerator.GenerateClientModel(clientBalance);
        var vehicle = DataGenerator.GenerateVehicleModel(rentCost);

        var httpClient = new HttpClientMockBuilder()
            .BuildHttpClient(client, vehicle);

        _httpClientFactoryMock
            .CreateClient(httpClient);

        _repositoryMock.GetById(vehicleClientHistory);

        var service = new VehicleClientHistoryService(_repositoryMock.Object,
            _distributedCacheMock.Object,
            _httpClientFactoryMock.Object,
            _configurationMock.Object
            );

        //Act
        var response = await service.UpdateAsync(correctUpdatedModel, default);

        //Assert
        response.Should().BeEquivalentTo(correctUpdatedModel);
    }

    [Fact]
    public async Task UpdateAsync_InvalidEndDateTime_ThrowsBadRequestException()
    {
        //Arrange
        var vehicleClientHistory = DataGenerator.GenerateVehicleClientHistoryData(1)[0];
        vehicleClientHistory.EndDate = DateTime.UtcNow;

        var correctUpdatedModel = vehicleClientHistory.Adapt<VehicleClientHistoryModel>();

        correctUpdatedModel.EndDate = DateTime.UtcNow - TimeSpan.FromDays(4);

        var service = new VehicleClientHistoryService(_repositoryMock.Object,
            _distributedCacheMock.Object,
            _httpClientFactoryMock.Object,
            _configurationMock.Object
            );

        //Act
        var response = async () => await service.UpdateAsync(correctUpdatedModel, default);

        //Assert
        await response.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task UpdateAsync_InvalidClientServiceResponse_ThrowsServiceException()
    {
        //Arrange
        var vehicleClientHistory = DataGenerator.GenerateVehicleClientHistoryData(1)[0];
        vehicleClientHistory.EndDate = DateTime.UtcNow;

        var correctUpdatedModel = vehicleClientHistory.Adapt<VehicleClientHistoryModel>();

        correctUpdatedModel.EndDate = DateTime.UtcNow + TimeSpan.FromDays(4);

        var totalRentDays = (correctUpdatedModel.EndDate - correctUpdatedModel.StartDate).TotalDays;

        var clientBalance = 9000 * Convert.ToDecimal(totalRentDays);

        var client = DataGenerator.GenerateClientModel(clientBalance);
        var vehicle = DataGenerator.GenerateVehicleModel(1000);

        var httpClient = new HttpClientMockBuilder()
            .BuildHttpClient(client, vehicle, clientPutResponseStatus: HttpStatusCode.InternalServerError);

        _httpClientFactoryMock
            .CreateClient(httpClient);

        _repositoryMock.GetById(vehicleClientHistory);

        var service = new VehicleClientHistoryService(_repositoryMock.Object,
            _distributedCacheMock.Object,
            _httpClientFactoryMock.Object,
            _configurationMock.Object
            );

        //Act
        var response = async () => await service.UpdateAsync(correctUpdatedModel, default);

        //Assert
        await response.Should().ThrowAsync<ServiceException>();
    }

    [Fact]
    public async Task UpdateAsync_InvalidId_ThrowsNotFoundException()
    {
        //Arrange
        _repositoryMock.GetByIdThrowsException();

        var correctUpdatedModel = _vehicleClientHistories[1].Adapt<VehicleClientHistoryModel>();
        var service = new VehicleClientHistoryService(_repositoryMock.Object,
            _distributedCacheMock.Object,
            _httpClientFactoryMock.Object,
            _configurationMock.Object
            );

        //Act
        var response = async () => await service.UpdateAsync(correctUpdatedModel, default);

        //Assert
        await response.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task DeleteAsync_VehicleClientHistoryId_()
    {
        //Arrange
        var service = new VehicleClientHistoryService(_repositoryMock.Object,
            _distributedCacheMock.Object,
            _httpClientFactoryMock.Object,
            _configurationMock.Object
            );

        //Act
        var response = async () => await service.DeleteAsync(Guid.NewGuid(), default);

        //Assert
        await response.Should().NotThrowAsync();
    }

    [Fact]
    public async Task DeleteAsync_InvalidId_ThrowsNotFoundException()
    {
        //Arrange
        _repositoryMock.GetByIdThrowsException();

        var service = new VehicleClientHistoryService(_repositoryMock.Object,
            _distributedCacheMock.Object,
            _httpClientFactoryMock.Object,
            _configurationMock.Object
            );

        //Act
        var response = async () => await service.DeleteAsync(Guid.NewGuid(), default);

        //Assert
        await response.Should().ThrowAsync<InvalidOperationException>();
    }
}