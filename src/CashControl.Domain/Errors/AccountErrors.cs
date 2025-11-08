using CashControl.Domain.Primitives;

namespace CashControl.Domain.Errors;

public class AccountErrors
{
    public static Error AccountNotFound(Guid id) =>
        new("ACCOUNT_NOT_FOUND", $"Account with ID '{id}' was not found.");
}
