using CashControl.Domain.Accounts;
using CashControl.Domain.Categories;
using CashControl.Domain.Enums;
using CashControl.Domain.Transactions;
using CashControl.Domain.ValueObjects;
using Xunit;

namespace CashControl.UnitTests.Entities.Accounts;

public class AccountTests
{
    [Fact(DisplayName = "Should set balance to zero when account is created")]
    public void Should_SetBalanceToZero_When_AccountIsCreated()
    {
        // Act
        Account account = Account.Create("Test Account");

        // Assert
        Assert.Equal(decimal.Zero, account.Balance.Value);
        Assert.Equal(Currency.BRL, account.Balance.Currency);
        Assert.Empty(account.Transactions);
    }

    [Theory(DisplayName = "Should add transaction and update balance")]
    [InlineData(100.00, TransactionType.Income, 100.00)]
    [InlineData(100.00, TransactionType.Expense, -100.00)]
    public void Should_AddTransactionAndUpdateBalance(
        decimal amount,
        TransactionType type,
        decimal expectedBalance
    )
    {
        // Arrange
        Category category = Category.Create("Test Category");
        Account account = Account.Create("Test Account");
        var transaction = Transaction.Create(
            category.Id,
            "Test Transaction",
            Money.Create(amount, Currency.BRL),
            type,
            DateTime.Now
        );

        // Act
        account.AddTransaction(transaction);

        // Assert
        Assert.Equal(1, account.Transactions.Count);
        Assert.Equal(expectedBalance, account.Balance.Value);
    }
}
