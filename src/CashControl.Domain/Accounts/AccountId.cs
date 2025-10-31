using CashControl.Domain.Primitives;

namespace CashControl.Domain.Accounts;

public class AccountId : EntityId<Guid>
{
    private AccountId() { }

    public AccountId(Guid value) : base(value) { }

    public static AccountId Create(Guid value) => new(value);
}