using CashControl.Domain.Enums;
using CashControl.Domain.Primitives;
using CashControl.Domain.Transactions;
using CashControl.Domain.ValueObjects;

namespace CashControl.Domain.Accounts;

public class Account : Entity<AccountId>
{
    private readonly List<Transaction> _transactions = [];

    protected Account() { }

    protected Account(string name, Currency currency)
    {
        Id = AccountId.CreateNew();
        Name = name;
        Currency = currency;
        Balance = Money.Zero(currency);
    }

    public string Name { get; private set; } = string.Empty;
    public Money Balance { get; private set; } = Money.Zero();
    public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();
    public Currency Currency { get; private set; }

    public static Account Create(string name, Currency currency = Currency.BRL) => new(name, currency);

    public void AddTransaction(Transaction transaction)
    {
        _transactions.Add(transaction);
        UpdateBalance(transaction);
    }

    private void UpdateBalance(Transaction transaction) =>
        Balance = transaction.Type switch
        {
            TransactionType.Income => Balance.Add(transaction.Amount),
            TransactionType.Expense => Balance.Subtract(transaction.Amount),
            _ => throw new ArgumentOutOfRangeException(
                typeof(TransactionType).Name,
                "Invalid transaction type"
            ),
        };
}
