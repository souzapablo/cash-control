using CashControl.App.Abstractions;

namespace CashControl.App.Features.Users;

public static class UserErrors
{
    public static Error EmailAlreadyRegistered => new("EMAIL_ALREADY_REGISTERED", "An user is already registered with this email.");
}
