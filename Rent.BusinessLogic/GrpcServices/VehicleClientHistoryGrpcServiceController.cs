using Grpc.Core;
using Mapster;
using Rent.BusinessLogic.Services.Interfaces;
using RentGrpcService;

namespace Rent.BusinessLogic.GrpcServices;

public class VehicleClientHistoryGrpcServiceController(IVehicleClientHistoryService service) : RentService.RentServiceBase
{
    public override async Task<GetVehicleClientHistoriesInRangeResponse> GetVehicleClientHistories(GetVehicleClientHistoriesInRangeRequest request, ServerCallContext context)
    {
        var dataList = await service.GetRangeAsync(request.PageNumber, request.PageSize, context.CancellationToken);

        var responseData = dataList.Adapt<IEnumerable<ProtoVehicleClientHistoryModel>>();

        var response = new GetVehicleClientHistoriesInRangeResponse();

        response.VehicleClientHistories.AddRange(responseData);

        return response;
    }

    public override async Task<GetVehicleClientHistoryResponse> GetVehicleClientHistory(GetVehicleClientHistoryRequest request, ServerCallContext context)
    {
        var idIsValid = Guid.TryParse(request.Id, out var id);

        if (!idIsValid)
            throw new InvalidOperationException("Provided id is not GUID");

        var data = await service.GetByIdAsync(id, context.CancellationToken);

        var responseData = data.Adapt<ProtoVehicleClientHistoryModel>();

        var response = new GetVehicleClientHistoryResponse
        {
            VehicleClientHistory = responseData
        };

        return response;
    }
}