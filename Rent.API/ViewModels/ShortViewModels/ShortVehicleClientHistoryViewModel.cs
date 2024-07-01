namespace Rent.API.ViewModels.ShortViewModels;

public record ShortVehicleClientHistoryViewModel(
    DateTime StartDate,
    DateTime EndDate,
    Guid VehicleId,
    Guid ClientId
    );