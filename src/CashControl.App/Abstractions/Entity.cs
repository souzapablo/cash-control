namespace CashControl.App.Abstractions;

public abstract class Entity<TId> : IEquatable<Entity<TId>> where TId : EntityId<TId>
{
    protected Entity(TId id)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
    }

    public TId Id { get; }
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public DateTime? LastUpdate { get; private set; }
    public bool IsActive { get; private set; } = true;

    public override bool Equals(object? obj)
    {
        if (obj is null || obj is not Entity<TId> other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return Id.Equals(other.Id);
    }

    public bool Equals(Entity<TId>? other) => Equals((object?)other);

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right) =>
        left is null && right is null || left is not null && left.Equals(right);

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right) => !(left == right);
    protected void Update() => LastUpdate = DateTime.UtcNow;
    public void Deactivate()
    {
        IsActive = false;
        Update();
    }

    public void Activate()
    {
        IsActive = true;
        Update();
    }
}
