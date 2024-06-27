using Moq;
using Rent.DataAccess.Entities;
using Rent.DataAccess.Repositories.Interfaces;
using System.Linq.Expressions;

namespace Rent.UnitTests.Mocks;

internal class VehicleClientHistoryRepositoryMock : Mock<IVehicleClientHistoryRepository>
{
    private readonly CancellationToken _anyToken = It.IsAny<CancellationToken>();

    public void GetRange(IEnumerable<VehicleClientHistoryEntity> vehicleClientHistoriesToReturn) =>
        Setup(cr => cr.GetRangeAsync(It.IsAny<int>(), It.IsAny<int>(), _anyToken))
            .ReturnsAsync(vehicleClientHistoriesToReturn);

    public void GetById(VehicleClientHistoryEntity? vehicleClientHistoryToReturn) =>
        Setup(cr => cr.GetByIdAsync(It.IsAny<Guid>(), _anyToken))
            .ReturnsAsync(vehicleClientHistoryToReturn);

    public void IsExists(bool boolToReturn) =>
        Setup(cr => cr.IsExistsAsync(It.IsAny<Expression<Func<VehicleClientHistoryEntity, bool>>>(), _anyToken))
        .ReturnsAsync(boolToReturn);
}