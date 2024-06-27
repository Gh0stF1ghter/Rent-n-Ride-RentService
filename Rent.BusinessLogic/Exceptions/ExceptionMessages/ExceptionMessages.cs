namespace Rent.BusinessLogic.Exceptions.ExceptionMessages;

internal static class ExceptionMessages
{
    public static string NotFound(string entityType, Guid guid) =>
        $"{entityType} with id:{guid} was not found";
}