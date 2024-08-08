namespace EventBus.CatalogueServiceEvents.CarModelEvents;

public record CarModelUpdated(
    Guid Id,
    string Name,
    Guid ManufacturerId
    );