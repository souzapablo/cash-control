using CashControl.Domain.Primitives;

namespace CashControl.Domain.Accounts;

public class Account : Entity<AccountId>
{
    protected Account() { }
    
    protected Account(string name)
    {
        Name = name;
    }

    public string Name { get; private set; } = string.Empty;
    public decimal Balance { get; private set; }

    public static Account Create(string name) =>
        new(name);
}