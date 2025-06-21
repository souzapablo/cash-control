namespace CashControl.API.Abstractions;

public interface IUnitOfWork
{
    Task CommitAsync(CancellationToken cancellationToken);
}