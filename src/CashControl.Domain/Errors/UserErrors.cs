using CashControl.Domain.Primitives;

namespace CashControl.Domain.Errors;

public class UserErrors
{
    public static Error EmailAlreadyExists =>
        new("EMAIL_ALREADY_EXISTS", "A user with the given email already exists.");

    public static Error InvalidCredentials =>
        new("INVALID_CREDENTIALS", "Invalid email or password.");
}
