using CashControl.Infrastructure.Data;
using CashControl.IntegrationTests.Infrastructure;

namespace CashControl.IntegrationTests.Extensions;

public static class DbContextExtensions
{
    public static async Task SeedDataAsync(this CashControlDbContext context)
    {
        Data.DefaultAccount.AddTransaction(Data.DefaultTransaction);

        if (!context.Categories.Any())
            context.Categories.Add(Data.DefaultCategory);

        if (!context.Accounts.Any())
        {
            Data.DefaultAccount.AddTransaction(Data.DefaultTransaction);

            context.Accounts.Add(Data.DefaultAccount);
        }
        await context.SaveChangesAsync();
    }
}
