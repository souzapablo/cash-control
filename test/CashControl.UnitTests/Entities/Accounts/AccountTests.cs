using System;
using CashControl.Domain.Accounts;
using CashControl.Domain.Enums;
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
        Assert.Equal(decimal.Zero, account.Balance.Amount);
        Assert.Equal(Currency.BRL, account.Balance.Currency);
    }
}
