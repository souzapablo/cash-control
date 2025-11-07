using CashControl.Domain.Primitives;

namespace CashControl.Domain.Transactions;

public class TransactionId : EntityId<Guid>
{
    protected TransactionId() { }

    public TransactionId(Guid value) : base(value) { }

    public static TransactionId Create(Guid value) => new(value);
    public static TransactionId CreateNew() => new(Guid.NewGuid());
}
