namespace Rent.BusinessLogic.Exceptions.ExceptionMessages;

internal static class ExceptionMessages
{
    public static string NotFound(string entityType, Guid guid) =>
        $"{entityType} with id:{guid} was not found";

    public static string InsufficientFunds(Guid userId, decimal userBalance, decimal vehicleCost) =>
        $"User with id:{userId} does not have enough funds on balance. User: {userBalance}, Vehicle: {vehicleCost}";
    public static string NewEndDateLessThanCurrent(DateTime newEndDateTime, DateTime currentEndDateTime) =>
        $"New end datetime is less than updated ({newEndDateTime} against {currentEndDateTime})";

    public static string NotFoundInService(string entityType, string? connection) =>
        $"{entityType} was not found while requesting {connection}";

    public static string UserIsRenting(Guid id) =>
        $"User ({id}) is already renting a vehicle";

    public static string VehicleIsRented(Guid id) =>
        $"Vehicle ({id}) is already rented";

    public static string ServiceError(string? connection) =>
        "Something went wrong during request: " + connection;
}