using CashControl.API.Abstractions;

namespace CashControl.API.Data.Persistence;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public Task CommitAsync(CancellationToken cancellationToken) =>  context.SaveChangesAsync(cancellationToken);
}