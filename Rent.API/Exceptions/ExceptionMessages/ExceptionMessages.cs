namespace Rent.API.Exceptions.ExceptionMessages;

public static class ExceptionMessages
{
    public const string StartDateGreaterThanCurrentDate = "Start date cannot be greater than current date";
    public const string EndDateLessThanStartDate = "End date cannot be less than or equal to start date";
    public const string NoVehicleId = "History should have a VehicleId";
    public const string NoClientId = "History should have a ClientId";
}