namespace CashControl.Domain.Primitives;

public abstract class Entity<TId>
    where TId : notnull
{
    protected Entity() { }

    protected Entity(TId id)
    {
        Id = id;
    }

    public TId Id { get; protected set; } = default!;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? LastUpdate { get; private set; }
    public bool IsActive { get; private set; } = true;

    public void Delete()
    {
        IsActive = false;
        LastUpdate = DateTime.UtcNow;
    }
}
