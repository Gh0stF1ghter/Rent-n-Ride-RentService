using EventBus.VehicleClientHistoryEvents;
using Mapster;
using MassTransit;
using Rent.BusinessLogic.Models;
using Rent.BusinessLogic.Services.Interfaces;

namespace Rent.BusinessLogic.Consumers;

public class VehicleClientHistoryUpdatedConsumer(IVehicleClientHistoryService service) : IConsumer<VehicleClientHistoryUpdated>
{
    public async Task Consume(ConsumeContext<VehicleClientHistoryUpdated> context)
    {
        var vehicleClientHistoryFromEvent = context.Message;

        await Console.Out.WriteLineAsync($"vehicle client history {vehicleClientHistoryFromEvent.VehicleId} consumed to create");

        var vehicleClientHistory = vehicleClientHistoryFromEvent.Adapt<VehicleClientHistoryModel>();

        await service.UpdateAsync(vehicleClientHistory, default);
    }
}