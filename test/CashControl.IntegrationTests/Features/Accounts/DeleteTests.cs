using System.Net;
using CashControl.Domain.Accounts;
using CashControl.Domain.Errors;
using CashControl.IntegrationTests.Extensions;
using CashControl.IntegrationTests.Infrastructure;
using CashControl.IntegrationTests.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CashControl.IntegrationTests.Features.Accounts;

public class DeleteTests : BaseIntegrationTest
{
    public DeleteTests(IntegrationTestWebAppFactory factory)
        : base(factory) { }

    [Fact(DisplayName = "Should return 204 No Content when account is deleted successfully")]
    public async Task Should_Return204NoContent_When_AccountIsDeletedSuccessfully()
    {
        // Arrange
        Guid accountId = await CreateAccountInDb("Test Account");

        // Act
        var response = await Client.DeleteAsync($"api/accounts/{accountId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact(DisplayName = "Should set IsActive to false when account is deleted successfully")]
    public async Task Should_SetIsActiveToFalse_When_AccountIsDeletedSuccessfully()
    {
        // Arrange
        Guid accountId = await CreateAccountInDb("Test Account");
        // Act
        await Client.DeleteAsync($"api/accounts/{accountId}");
        var deletedAccount = await GetAccountInDb(accountId);

        // Assert

        Assert.False(deletedAccount?.IsActive);
    }

    [Fact(DisplayName = "Should return 404 Not Found when account does not exist")]
    public async Task Should_Return404NotFound_When_AccountDoesNotExist()
    {
        // Arrange
        var accountId = Guid.NewGuid();

        // Act
        var response = await Client.DeleteAsync($"api/accounts/{accountId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName = "Should return AccountNotFoundError when account does not exist")]
    public async Task Should_ReturnAccountNotFoundError_When_AccountDoesNotExist()
    {
        // Arrange
        var accountId = Guid.NewGuid();

        // Act
        var response = await Client.DeleteAsync($"api/accounts/{accountId}");

        // Assert
        var result = response.ReadAsResultAsync<Result>();
        Assert.False(result?.IsSuccess);
        Assert.Equal(AccountErrors.AccountNotFound(accountId), result?.Error);
    }

    private async Task<Guid> CreateAccountInDb(string name)
    {
        Account account = Account.Create(name);
        Context.Accounts.Add(account);
        await Context.SaveChangesAsync();
        return account.Id.Value;
    }

    private async Task<Account?> GetAccountInDb(Guid? id)
    {
        AccountId accountId = new(id.GetValueOrDefault());
        Account? accountInDb = await Context
            .Accounts.AsNoTracking()
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(a => a.Id == accountId);
        return accountInDb;
    }
}
