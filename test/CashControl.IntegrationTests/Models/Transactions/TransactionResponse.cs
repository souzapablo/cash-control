using CashControl.Api.Responses.ValueObjects;
using CashControl.Domain.Transactions;

namespace CashControl.IntegrationTests.Models.Transactions;

public class TransactionsResponse
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public MoneyResponse Amount { get; set; } = null!;
    public TransactionType Type { get; set; }
    public DateTime Date { get; set; }
}
