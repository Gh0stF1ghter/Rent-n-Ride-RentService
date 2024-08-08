namespace EventBus.CatalogueServiceEvents.ManufacturerEvents;

public record ManufacturerUpdated(
    Guid Id,
    string Name
    );