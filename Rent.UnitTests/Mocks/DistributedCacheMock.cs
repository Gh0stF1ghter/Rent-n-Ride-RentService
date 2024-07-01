using Microsoft.Extensions.Caching.Distributed;
using Moq;

namespace Rent.UnitTests.Mocks;

internal class DistributedCacheMock : Mock<IDistributedCache>
{
    public void GetDataFromCache(byte[] dataToReturn) =>
        Setup(dc => dc.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(dataToReturn);
}
