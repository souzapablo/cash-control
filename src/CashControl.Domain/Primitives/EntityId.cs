namespace CashControl.Domain.Primitives;

public abstract class EntityId<TValue>
    where TValue : struct
{
    protected EntityId() { }

    protected EntityId(TValue value)
    {
        Value = value;
    }

    public TValue Value { get; protected init; }

    public override bool Equals(object? obj) =>
        obj is EntityId<TValue> other && Value.Equals(other.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public static implicit operator TValue(EntityId<TValue> id) => id.Value;
}
