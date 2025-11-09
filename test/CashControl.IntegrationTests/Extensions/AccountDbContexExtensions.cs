using CashControl.Domain.Accounts;
using CashControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CashControl.IntegrationTests.Extensions;

public static class AccountDbContexExtensions
{
    public static async Task<Account?> GetAccountByIdAsync(
        this CashControlDbContext context,
        Guid id
    )
    {
        AccountId accountId = AccountId.Create(id);
        Account? accountInDb = await context
            .Accounts.AsNoTracking()
            .Include(account => account.Transactions)
            .SingleOrDefaultAsync(a => a.Id == accountId);
        return accountInDb;
    }

    public static async Task<Account?> GetDeletedAccountByIdAsync(
        this CashControlDbContext context,
        Guid id
    )
    {
        AccountId accountId = AccountId.Create(id);
        Account? accountInDb = await context
            .Accounts.AsNoTracking()
            .IgnoreQueryFilters()
            .Include(account => account.Transactions)
            .SingleOrDefaultAsync(a => a.Id == accountId);
        return accountInDb;
    }

    public static async Task<Guid> CreateAccountAsync(
        this CashControlDbContext context,
        string name
    )
    {
        Account account = Account.Create(name);
        context.Accounts.Add(account);
        await context.SaveChangesAsync();
        return account.Id.Value;
    }
}
