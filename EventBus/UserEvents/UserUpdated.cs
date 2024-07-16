namespace EventBus.UserEvents;

public record UserUpdated(
    Guid Id,
    string FirstName,
    string LastName,
    bool IsRenting
    );