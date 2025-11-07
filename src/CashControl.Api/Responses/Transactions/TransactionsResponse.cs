using CashControl.Api.Responses.ValueObjects;
using CashControl.Domain.Transactions;

namespace CashControl.Api.Responses.Transactions;

public record TransactionsResponse(
    Guid Id,
    string Description,
    MoneyResponse Amount,
    TransactionType Type,
    DateTime Date
);
