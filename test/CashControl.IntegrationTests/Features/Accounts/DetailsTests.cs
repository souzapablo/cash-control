using System.Net;
using CashControl.Api.Feature.Accounts;
using CashControl.Domain.Accounts;
using CashControl.IntegrationTests.Extensions;
using CashControl.IntegrationTests.Infrastructure;
using CashControl.IntegrationTests.Models.Accounts;
using Xunit;

namespace CashControl.IntegrationTests.Features.Accounts;

public class DetailsTests : BaseIntegrationTest
{
    public DetailsTests(IntegrationTestWebAppFactory factory)
        : base(factory) { }

    [Fact(DisplayName = "Should return 200 Ok when account is found")]
    public async Task Should_ReturnOk_When_AccountIsFound()
    {
        // Arrange
        Account account = await CreateAccountInDb("Test Account");

        // Act
        var response = await Client.GetAsync($"/api/accounts/{account.Id.Value}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact(DisplayName = "Should return account values when account is found")]
    public async Task Should_ReturnAccountValues_When_AccountIsFound()
    {
        // Arrange
        Account account = await CreateAccountInDb("Test Account");

        // Act
        var response = await Client.GetAsync($"/api/accounts/{account.Id.Value}");
        var result = response.ReadAsResultAsync<AccountDetailsResponse>();

        // Assert
        Assert.Equal(account.Balance.Value, result?.Value?.Balance.Amount);
        Assert.Equal(account.Balance.Currency.ToString(), result?.Value?.Balance.Currency);
        Assert.Equal(account.Name, result?.Value?.Name);
        Assert.Equal(account.Id.Value, result?.Value?.Id);
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
        var result = response.ReadAsResultAsync<AccountDetailsResponse>();

        // Assert
        Assert.False(result?.IsSuccess);
        Assert.NotNull(result?.Error);
        Assert.Equal(result.Error, Errors.AccountNotFound(accountId));
    }

    private async Task<Account> CreateAccountInDb(string name)
    {
        Account account = Account.Create(name);
        Context.Accounts.Add(account);
        await Context.SaveChangesAsync();
        return account;
    }
}
