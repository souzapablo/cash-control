using CashControl.App.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CashControl.App.Features.Users;

public interface IUserRepository
{
    Task<bool> ExistWithEmailAsync(string email, CancellationToken cancellationToken = default);
    void Register(User user);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<bool> ExistWithEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await context.Users.AnyAsync(u => u.Email == email, cancellationToken);
    public void Register(User user) => context.Add(user);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await context.SaveChangesAsync(cancellationToken);
}
