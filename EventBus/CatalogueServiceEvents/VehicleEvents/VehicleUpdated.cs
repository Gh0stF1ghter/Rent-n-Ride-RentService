using EventBus.Enums;

namespace EventBus.CatalogueServiceEvents.VehicleEvents;

public record VehicleUpdated(
    Guid Id,
    Guid? ModelId,
    string PlateNumber,
    decimal RentCost,
    int Odo,
    bool IsRented,
    VehicleType VehicleType = VehicleType.None,
    VehicleState VehicleState = VehicleState.None,
    FuelType FuelType = FuelType.None
    );