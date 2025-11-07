using System.Transactions;
using CashControl.Domain.Primitives;
using CashControl.Domain.ValueObjects;

namespace CashControl.Domain.Accounts;

public class Account : Entity<AccountId>
{
    private readonly List<Transaction> _transactions = [];
    protected Account() { }
    
    protected Account(string name)
    {
        Id = AccountId.CreateNew();
        Name = name;
    }

    public string Name { get; private set; } = string.Empty;
    public Money Balance { get; private set; } = Money.Zero();
    public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();

    public static Account Create(string name) =>
        new(name);
}