using CashControl.IntegrationTests.Models.ValueObjects;

namespace CashControl.IntegrationTests.Models.Accounts;

public record class ListAccountsResponse(
    Guid Id,
    string Name,
    MoneyResponse Balance
);