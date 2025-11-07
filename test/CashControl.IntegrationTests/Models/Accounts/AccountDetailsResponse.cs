using CashControl.Api.Responses.Transactions;
using CashControl.IntegrationTests.Models.ValueObjects;

namespace CashControl.IntegrationTests.Models.Accounts;

public record AccountDetailsResponse(
    Guid Id,
    string Name,
    MoneyResponse Balance,
    IEnumerable<TransactionsResponse> Transactions
);
