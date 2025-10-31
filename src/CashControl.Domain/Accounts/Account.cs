using CashControl.Domain.Primitives;
using CashControl.Domain.ValueObjects;

namespace CashControl.Domain.Accounts;

public class Account : Entity<AccountId>
{
    protected Account() { }
    
    protected Account(string name)
    {
        Name = name;
    }

    public string Name { get; private set; } = string.Empty;
    public Money Balance { get; private set; } = Money.Zero();

    public static Account Create(string name) =>
        new(name);
}