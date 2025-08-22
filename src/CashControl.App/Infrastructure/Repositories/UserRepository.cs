using CashControl.App.Features.Users;
using CashControl.App.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CashControl.App.Infrastructure.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<bool> ExistWithEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await context.Users.AnyAsync(u => u.Email == email, cancellationToken);
        
    public void Register(User user) => context.Add(user);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await context.SaveChangesAsync(cancellationToken);
}
