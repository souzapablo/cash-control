using System;
using CashControl.Domain.Primitives;

namespace CashControl.Api.Feature.Accounts;

public class Errors
{
    public static Error AccountNotFound(Guid id) => new("ACCOUNT_NOT_FOUND", $"Account with ID '{id}' was not found.");
}
