namespace CashControl.API.Common;

public abstract class Entity
{
    public long Id { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? LastUpdate { get; private set; }
}