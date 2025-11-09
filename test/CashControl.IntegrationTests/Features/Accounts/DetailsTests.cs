using System.Net;
using CashControl.Domain.Accounts;
using CashControl.Domain.Errors;
using CashControl.IntegrationTests.Extensions;
using CashControl.IntegrationTests.Infrastructure;
using CashControl.IntegrationTests.Models.Accounts;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CashControl.IntegrationTests.Features.Accounts;

public class DetailsTests : BaseIntegrationTest
{
    public DetailsTests(IntegrationTestWebAppFactory factory)
        : base(factory) { }

    [Fact(DisplayName = "Should return 200 Ok when account is found")]
    public async Task Should_ReturnOk_When_AccountIsFound()
    {
        // Act
        var response = await Client.GetAsync($"/api/accounts/{Data.DefaultAccount.Id.Value}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact(DisplayName = "Should return account values when account is found")]
    public async Task Should_ReturnAccountValues_When_AccountIsFound()
    {
        // Act
        Account? defaultAccount = await Context.Accounts.SingleOrDefaultAsync(account =>
            account.Id == Data.DefaultAccount.Id
        );
        var response = await Client.GetAsync($"/api/accounts/{Data.DefaultAccount.Id.Value}");
        var result = await response.ReadAsResultAsync<AccountDetailsResponse>();

        // Assert
        Assert.Equal(defaultAccount?.Balance.Value, result?.Value?.Balance.Amount);
        Assert.Equal(defaultAccount?.Balance.Currency.ToString(), result?.Value?.Balance.Currency);
        Assert.Equal(defaultAccount?.Name, result?.Value?.Name);
        Assert.Equal(defaultAccount?.Id.Value, result?.Value?.Id);
    }

    [Fact(DisplayName = "Should return 404 Not Found when account is not found")]
    public async Task Should_ReturnNotFound_When_AccountIsNotFound()
    {
        // Arrange
        Guid accountId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/accounts/{accountId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName = "Should return AccountNotFound error when account is not found")]
    public async Task Should_ReturnAccountNotFoundError_When_AccountIsNotFound()
    {
        // Arrange
        Guid accountId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/accounts/{accountId}");
        var result = await response.ReadAsResultAsync<AccountDetailsResponse>();

        // Assert
        Assert.False(result?.IsSuccess);
        Assert.NotNull(result?.Error);
        Assert.Equal(result.Error, AccountErrors.AccountNotFound(accountId));
    }
}
