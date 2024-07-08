using Moq;
using Rent.BusinessLogic.Models;
using SoloX.CodeQuality.Test.Helpers.Http;
using System.Net;

namespace Rent.UnitTests.Mocks;

internal static class HttpClientMockBuilderExtension
{
    private static readonly string _clientConnection = nameof(ClientModel) + It.IsAny<Guid>();
    private static readonly string _vehicleConnection = nameof(VehicleModel) + It.IsAny<Guid>();

    public static HttpClient BuildHttpClient(
        this HttpClientMockBuilder httpClient,
        ClientModel clientGetResponse,
        VehicleModel vehicleGetResponse,
        HttpStatusCode clientGetResponseStatus = HttpStatusCode.OK,
        HttpStatusCode vehicleGetResonseStatus = HttpStatusCode.OK,
        HttpStatusCode clientPutResponseStatus = HttpStatusCode.OK,
        HttpStatusCode vehiclePutResonseStatus = HttpStatusCode.OK
        ) =>
        httpClient.WithBaseAddress(new("http://localhost:5001"))
            .WithRequest(_clientConnection, HttpMethod.Get)
            .RespondingJsonContent(clientGetResponse, clientGetResponseStatus)
            .WithRequest(_vehicleConnection, HttpMethod.Get)
            .RespondingJsonContent(vehicleGetResponse, vehicleGetResonseStatus)
            .WithRequest(_clientConnection, HttpMethod.Put)
            .RespondingStatus(clientPutResponseStatus)
            .WithRequest(_vehicleConnection, HttpMethod.Put)
            .RespondingStatus(vehiclePutResonseStatus)
            .Build();
}
