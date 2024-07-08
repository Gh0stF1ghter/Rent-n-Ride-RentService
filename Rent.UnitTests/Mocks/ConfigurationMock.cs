using Microsoft.Extensions.Configuration;
using Moq;
using Rent.BusinessLogic.Models;

namespace Rent.UnitTests.Mocks;
internal class ConfigurationMock : Mock<IConfiguration>
{
    public void GetUserConnection() =>
        Setup(c => c.GetSection("ConnectionStrings")["UserServiceConnection"])
        .Returns(nameof(ClientModel));

    public void GetCatalogueCoonnection() =>
        Setup(c => c.GetSection("ConnectionStrings")["CatalogueServiceConnection"])
        .Returns(nameof(VehicleModel));
}