namespace EventBus.UserEvents;

public record UserCreated(
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    decimal Balance,
    bool IsRenting
    );