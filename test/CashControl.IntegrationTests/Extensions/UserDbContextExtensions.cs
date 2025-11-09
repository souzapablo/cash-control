using CashControl.Domain.Users;
using CashControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CashControl.IntegrationTests.Extensions;

public static class UserDbContextExtensions
{
    public static async Task<User?> GetUserByIdAsync(this CashControlDbContext context, Guid id)
    {
        UserId userId = UserId.Create(id);
        User? userInDb = await context
            .Users.AsNoTracking()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == userId);
        return userInDb;
    }

    public static async Task<User?> GetUserByEmailAsync(
        this CashControlDbContext context,
        string email
    )
    {
        User? userInDb = await context
            .Users.AsNoTracking()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email.Address == email);
        return userInDb;
    }
}
