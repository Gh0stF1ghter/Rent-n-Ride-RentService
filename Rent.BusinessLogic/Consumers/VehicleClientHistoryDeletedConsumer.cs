using EventBus.VehicleClientHistoryEvents;
using MassTransit;
using Rent.BusinessLogic.Services.Interfaces;

namespace Rent.BusinessLogic.Consumers;

public class VehicleClientHistoryDeletedConsumer(IVehicleClientHistoryService service) : IConsumer<VehicleClientHistoryDeleted>
{
    public async Task Consume(ConsumeContext<VehicleClientHistoryDeleted> context)
    {
        var vehicleClientHistoryId = context.Message.Id;

        await Console.Out.WriteLineAsync($"vehicle client history {vehicleClientHistoryId} consumed to delete");

        await service.DeleteAsync(vehicleClientHistoryId, default);
    }
}
