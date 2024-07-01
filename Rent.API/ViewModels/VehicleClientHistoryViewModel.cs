namespace Rent.API.ViewModels;

public record VehicleClientHistoryViewModel(
    Guid Id,
    DateTime StartDate,
    DateTime EndDate
    );