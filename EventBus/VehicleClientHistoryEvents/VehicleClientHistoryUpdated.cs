namespace EventBus.VehicleClientHistoryEvents;

public record VehicleClientHistoryUpdated(
    Guid Id,
    DateTime StartDate,
    DateTime EndDate,
    Guid VehicleId,
    Guid ClientId
    );