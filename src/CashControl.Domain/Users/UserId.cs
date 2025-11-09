using CashControl.Domain.Primitives;

namespace CashControl.Domain.Users;

public class UserId : EntityId<Guid>
{
    protected UserId() { }

    private UserId(Guid value)
        : base(value) { }

    public static UserId Create(Guid value) => new(value);

    public static UserId CreateNew() => new(Guid.NewGuid());
}
