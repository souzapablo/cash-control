namespace CashControl.Api.Abstractions;

public abstract class EntityId<T> : IEquatable<EntityId<T>> where T : EntityId<T>
{
    protected Guid Value { get; }

    protected EntityId(Guid value)
    {
        Value = value;
    }
    public static T New() => (T)Activator.CreateInstance(typeof(T), Guid.NewGuid())!;
    public static T From(Guid value) => (T)Activator.CreateInstance(typeof(T), value)!;
    
    public override bool Equals(object? obj) => obj is EntityId<T> other && Equals(other);

    public bool Equals(EntityId<T>? other) => other is not null && Value.Equals(other.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool operator ==(EntityId<T>? a, EntityId<T>? b) =>
        a is null && b is null || a is not null && a.Equals(b);

    public static bool operator !=(EntityId<T>? a, EntityId<T>? b) => !(a == b);
}