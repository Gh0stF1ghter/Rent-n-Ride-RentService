using Microsoft.Extensions.Configuration;
using Moq;

namespace Rent.UnitTests.Mocks;
internal class ConfigurationMock : Mock<IConfiguration>
{
    public void GetUserConnection() =>
        Setup(c => c.GetSection("ConnectionStrings")["UserServiceConnection"])
        .Returns("http://localhost:5054/api/client/");

    public void GetCatalogueCoonnection() =>
        Setup(c => c.GetSection("ConnectionStrings")["CatalogueServiceConnection"])
        .Returns("http://localhost:5131/api/vehicle/");
}