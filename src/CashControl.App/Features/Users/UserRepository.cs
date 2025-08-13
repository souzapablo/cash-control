using CashControl.App.Infrastructure.Data;

namespace CashControl.App.Features.Users;

public interface IUserRepository
{
    void Register(User user);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

public class UserRepository(AppDbContext context) : IUserRepository
{
    public void Register(User user) => context.Add(user);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await context.SaveChangesAsync(cancellationToken);
}
