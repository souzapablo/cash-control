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
}
