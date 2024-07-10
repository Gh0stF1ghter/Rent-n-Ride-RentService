using Moq;

namespace Rent.UnitTests.Mocks;

internal class HttpClientFactoryMock : Mock<IHttpClientFactory>
{
    public void CreateClient(HttpClient httpClient) =>
        Setup(_ => _.CreateClient(It.IsAny<string>()))
        .Returns(httpClient);
}