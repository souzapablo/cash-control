using System.Net;
using CashControl.Domain.Accounts;
using CashControl.IntegrationTests.Extensions;
using CashControl.IntegrationTests.Infrastructure;
using CashControl.IntegrationTests.Models.Accounts;
using Xunit;

namespace CashControl.IntegrationTests.Features.Accounts;

public class ListTests : BaseIntegrationTest
{
    public ListTests(IntegrationTestWebAppFactory factory)
        : base(factory) { }

    [Fact(DisplayName = "Should return 200 OK when accounts exist")]
    public async Task Should_ReturnOk_When_AccountsExist()
    {
        // Arrange
        await CreateAccountInDb("Account 1");
        await CreateAccountInDb("Account 2");

        // Act
        var response = await Client.GetAsync("/api/accounts");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact(DisplayName = "Should return empty list when no accounts exist")]
    public async Task Should_ReturnEmptyList_When_NoAccountsExist()
    {
        // Act
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
        Account activeAccount2 = await CreateAccountInDb("Active Account 2");
        Account deletedAccount = await CreateAccountInDb("Deleted Account");
        
        await Client.DeleteAsync($"/api/accounts/{deletedAccount.Id.Value}");

        // Act
        var response = await Client.GetAsync("/api/accounts");
        var result = response.ReadAsResultAsync<IEnumerable<ListAccountsResponse>>();

        
        var accounts = result?.Value.ToList();
        Assert.Equal(2, accounts?.Count);
        Assert.Contains(accounts!, a => a.Id == activeAccount1.Id.Value);
        Assert.Contains(accounts!, a => a.Id == activeAccount2.Id.Value);
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
        Assert.Equal(account.Balance.Amount, returnedAccount.Balance.Amount);
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

