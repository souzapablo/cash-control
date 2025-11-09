using CashControl.Domain.Transactions;
using CashControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CashControl.IntegrationTests.Extensions;

public static class TransactionDbContextExtensions
{
    public static async Task<Transaction?> GetTransactionByIdAsync(
        this CashControlDbContext context,
        Guid? id
    )
    {
        if (id is null)
            return null;

        TransactionId transactionId = TransactionId.Create(id.Value);
        Transaction? transaction = await context
            .Transactions.Include(transaction => transaction.Category)
            .FirstOrDefaultAsync(t => t.Id == transactionId);
        return transaction;
    }
}
