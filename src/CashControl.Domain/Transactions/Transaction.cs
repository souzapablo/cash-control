using CashControl.Domain.Accounts;
using CashControl.Domain.Primitives;
using CashControl.Domain.ValueObjects;

namespace CashControl.Domain.Transactions;

public class Transaction : Entity<TransactionId>
{
    protected Transaction() { }

    private Transaction(string description, Money amount, TransactionType type, DateTime date)
    {
        Id = TransactionId.CreateNew();
        Description = description;
        Amount = amount;
        Type = type;
        Date = date;
    }

    public AccountId AccountId { get; private set; } = default!;
    public string Description { get; private set; } = string.Empty;
    public Money Amount { get; private set; } = Money.Zero();
    public TransactionType Type { get; private set; }
    public DateTime Date { get; private set; }

    public static Transaction Create(
        string description,
        Money amount,
        TransactionType type,
        DateTime date
    ) => new(description, amount, type, date);
}
