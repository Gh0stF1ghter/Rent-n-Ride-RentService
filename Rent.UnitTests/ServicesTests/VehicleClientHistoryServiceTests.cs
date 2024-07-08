using FluentAssertions;
using Mapster;
using Newtonsoft.Json;
using Rent.BusinessLogic.Models;
using Rent.BusinessLogic.Services.Implementations;
using Rent.DataAccess.Entities;
using Rent.UnitTests.DataGeneration;
using Rent.UnitTests.Mocks;
using System.Text;

namespace Rent.UnitTests.ServicesTests;
public class VehicleClientHistoryServiceTests
{
    private readonly VehicleClientHistoryRepositoryMock _repositoryMock = new();

    private readonly DistributedCacheMock _distributedCacheMock = new();

    private readonly List<VehicleClientHistoryEntity> _vehicleClientHistories = DataGenerator.AddVehicleClientHistoryData(5);

    public VehicleClientHistoryServiceTests()
    {
        _repositoryMock.GetRange(_vehicleClientHistories);
        _repositoryMock.GetById(_vehicleClientHistories[0]);
        _repositoryMock.IsExists(true);
    }

    [Fact]
    public async Task GetRangeAsync__ReturnsVehicleClientHistoryModelList()
    {
        //Arrange
        var correctModels = _vehicleClientHistories.Adapt<IEnumerable<VehicleClientHistoryModel>>();

        var service = new VehicleClientHistoryService(_repositoryMock.Object, _distributedCacheMock.Object);

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

        var service = new VehicleClientHistoryService(_repositoryMock.Object, _distributedCacheMock.Object);

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

        var service = new VehicleClientHistoryService(_repositoryMock.Object, _distributedCacheMock.Object);

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

        var service = new VehicleClientHistoryService(_repositoryMock.Object, _distributedCacheMock.Object);

        //Act
        var response = await service.GetByIdAsync(Guid.NewGuid(), default);

        //Assert
        response.Should().BeEquivalentTo(correctModel);
    }

    [Fact]
    public async Task AddAsync_VehicleClientHistoryModel_ReturnsVehicleClientHistoryModel()
    {
        //Arrange
        var correctModel = _vehicleClientHistories[0].Adapt<VehicleClientHistoryModel>();
        var service = new VehicleClientHistoryService(_repositoryMock.Object, _distributedCacheMock.Object);

        //Act
        var response = await service.AddAsync(correctModel, default);

        //Assert
        response.Should().BeEquivalentTo(correctModel);
    }

    [Fact]
    public async Task UpdateAsync_VehicleClientHistoryModel_ReturnsVehicleClientHistoryModel()
    {
        //Arrange
        var correctUpdatedModel = _vehicleClientHistories[1].Adapt<VehicleClientHistoryModel>();
        var service = new VehicleClientHistoryService(_repositoryMock.Object, _distributedCacheMock.Object);

        //Act
        var response = await service.UpdateAsync(correctUpdatedModel, default);

        //Assert
        response.Should().BeEquivalentTo(correctUpdatedModel);
    }

    [Fact]
    public async Task UpdateAsync_InvalidId_ThrowsNotFoundException()
    {
        //Arrange
        _repositoryMock.GetByIdThrowsException();

        var correctUpdatedModel = _vehicleClientHistories[1].Adapt<VehicleClientHistoryModel>();
        var service = new VehicleClientHistoryService(_repositoryMock.Object, _distributedCacheMock.Object);

        //Act
        var response = async () => await service.UpdateAsync(correctUpdatedModel, default);

        //Assert
        await response.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task DeleteAsync_VehicleClientHistoryId_()
    {
        //Arrange
        var service = new VehicleClientHistoryService(_repositoryMock.Object, _distributedCacheMock.Object);

        //Act
        var response = async () => await service.DeleteAsync(Guid.NewGuid(), default);

        //Assert
        await response.Should().NotThrowAsync();
    }

    [Fact]
    public async Task DeleteAsync_InvalidId_ThrowsNotFoundException()
    {
        //Arrange
        _repositoryMock.RemoveByIdThrowsException();

        var service = new VehicleClientHistoryService(_repositoryMock.Object, _distributedCacheMock.Object);

        //Act
        var response = async () => await service.DeleteAsync(Guid.NewGuid(), default);

        //Assert
        await response.Should().ThrowAsync<InvalidOperationException>();
    }
}