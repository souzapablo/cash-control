using CashControl.App.Abstractions;

namespace CashControl.App.Features.Users;

public static class UserErrors
{
    public static Error PepperNotFound => new("PEPPER_NOT_FOUND", "The pepper could not be found.");
    public static Error EmailAlreadyRegistered => new("EMAIL_ALREADY_REGISTERED", "A user is already registered with this email.");
}