using System.Net;
using CashControl.Domain.Accounts;
using CashControl.IntegrationTests.Extensions;
using CashControl.IntegrationTests.Infrastructure;
using CashControl.IntegrationTests.Models.Accounts;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CashControl.IntegrationTests.Features.Accounts;

public class ListTests : BaseIntegrationTest
{
    public ListTests(IntegrationTestWebAppFactory factory)
        : base(factory) { }

    [Fact(DisplayName = "Should return 200 OK when accounts exist")]
    public async Task Should_ReturnOk_When_AccountsExist()
    {
        // Act
        var response = await Client.GetAsync("/api/accounts");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact(DisplayName = "Should return empty list when no accounts exist")]
    public async Task Should_ReturnEmptyList_When_NoAccountsExist()
    {
        // Act
        await Context.Database.ExecuteSqlRawAsync("DELETE FROM accounts;");
        var response = await Client.GetAsync("/api/accounts");
        var result = response.ReadAsResultAsync<IEnumerable<ListAccountsResponse>>();

        // Assert
        Assert.True(result?.IsSuccess);
        Assert.NotNull(result?.Value);
        Assert.Empty(result.Value);
    }

    [Fact(DisplayName = "Should return only active accounts")]
    public async Task Should_ReturnOnlyActiveAccounts_When_InactiveAccountsExist()
    {
        // Arrange
        Account activeAccount1 = await CreateAccountInDb("Active Account 1");
        Account deletedAccount = await CreateAccountInDb("Deleted Account");
        var countResponse = await Client.GetAsync("/api/accounts");
        var countResult = countResponse.ReadAsResultAsync<IEnumerable<ListAccountsResponse>>();

        await Client.DeleteAsync($"/api/accounts/{deletedAccount.Id.Value}");

        // Act
        var response = await Client.GetAsync("/api/accounts");
        var result = response.ReadAsResultAsync<IEnumerable<ListAccountsResponse>>();

        // Assert
        var accounts = result?.Value.ToList();
        var activeAccounts = countResult?.Value.Count() - 1;
        Assert.Equal(activeAccounts, accounts?.Count);
        Assert.Contains(accounts!, a => a.Id == activeAccount1.Id.Value);
        Assert.DoesNotContain(accounts!, a => a.Id == deletedAccount.Id.Value);
    }

    [Fact(DisplayName = "Should return correct account data structure")]
    public async Task Should_ReturnCorrectAccountDataStructure_When_AccountsExist()
    {
        // Arrange
        Account account = await CreateAccountInDb("Test Account");

        // Act
        var response = await Client.GetAsync("/api/accounts");
        var result = response.ReadAsResultAsync<IEnumerable<ListAccountsResponse>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result?.IsSuccess);
        Assert.NotNull(result?.Value);

        var accounts = result.Value.ToList();
        var returnedAccount = accounts.FirstOrDefault(a => a.Id == account.Id.Value);

        Assert.NotNull(returnedAccount);
        Assert.Equal(account.Id.Value, returnedAccount.Id);
        Assert.Equal(account.Name, returnedAccount.Name);
        Assert.Equal(account.Balance.Value, returnedAccount.Balance.Amount);
        Assert.Equal(account.Balance.Currency.ToString(), returnedAccount.Balance.Currency);
    }

    private async Task<Account> CreateAccountInDb(string name)
    {
        Account account = Account.Create(name);
        Context.Accounts.Add(account);
        await Context.SaveChangesAsync();
        return account;
    }
}
