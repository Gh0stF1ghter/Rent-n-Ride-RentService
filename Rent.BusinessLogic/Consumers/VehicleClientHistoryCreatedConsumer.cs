using EventBus.VehicleClientHistoryEvents;
using Mapster;
using MassTransit;
using Rent.BusinessLogic.Models;
using Rent.BusinessLogic.Services.Interfaces;

namespace Rent.BusinessLogic.Consumers;

public class VehicleClientHistoryCreatedConsumer(IVehicleClientHistoryService service) : IConsumer<VehicleClientHistoryCreated>
{
    public async Task Consume(ConsumeContext<VehicleClientHistoryCreated> context)
    {
        var vehicleClientHistoryFromEvent = context.Message;

        await Console.Out.WriteLineAsync($"vehicle client history {vehicleClientHistoryFromEvent.VehicleId} consumed to create");

        var vehicleClientHistory = vehicleClientHistoryFromEvent.Adapt<VehicleClientHistoryModel>();

        await service.AddAsync(vehicleClientHistory, default);
    }
}