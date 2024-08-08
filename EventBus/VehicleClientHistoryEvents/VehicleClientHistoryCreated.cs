namespace EventBus.VehicleClientHistoryEvents;

public record VehicleClientHistoryCreated(
    DateTime StartDate,
    DateTime EndDate,
    Guid VehicleId,
    Guid ClientId
    );