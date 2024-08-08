namespace EventBus.CatalogueServiceEvents.CarModelEvents;

public record CarModelCreated(
    string Name,
    Guid ManufacturerId
    );