using CashControl.Api.Responses.ValueObjects;

namespace CashControl.Api.Responses.Transactions;

public record TransactionsResponse(
    Guid Id,
    string Description,
    string Category,
    MoneyResponse Amount,
    string Type,
    DateTime Date
);
