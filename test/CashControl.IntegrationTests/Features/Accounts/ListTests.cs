using System.Net;
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
        var result = await response.ReadAsResultAsync<IEnumerable<ListAccountsResponse>>();

        // Assert
        Assert.True(result?.IsSuccess);
        Assert.NotNull(result?.Value);
        Assert.Empty(result.Value);
    }

    [Fact(DisplayName = "Should return only active accounts")]
    public async Task Should_ReturnOnlyActiveAccounts_When_InactiveAccountsExist()
    {
        // Arrange
        Guid activeAccount1 = await Context.CreateAccountAsync("Active Account 1");
        Guid deletedAccount = await Context.CreateAccountAsync("Deleted Account");
        var countResponse = await Client.GetAsync("/api/accounts");
        var countResult = await countResponse.ReadAsResultAsync<
            IEnumerable<ListAccountsResponse>
        >();

        await Client.DeleteAsync($"/api/accounts/{deletedAccount}");

        // Act
        var response = await Client.GetAsync("/api/accounts");
        var result = await response.ReadAsResultAsync<IEnumerable<ListAccountsResponse>>();

        // Assert
        var accounts = result?.Value.ToList();
        var activeAccounts = countResult?.Value.Count() - 1;
        Assert.Equal(activeAccounts, accounts?.Count);
        Assert.Contains(accounts!, a => a.Id == activeAccount1);
        Assert.DoesNotContain(accounts!, a => a.Id == deletedAccount);
    }

    [Fact(DisplayName = "Should return correct account data structure")]
    public async Task Should_ReturnCorrectAccountDataStructure_When_AccountsExist()
    {
        // Arrange
        Guid accountId = await Context.CreateAccountAsync("Test Account");

        // Act
        var response = await Client.GetAsync("/api/accounts");
        var result = await response.ReadAsResultAsync<IEnumerable<ListAccountsResponse>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result?.IsSuccess);
        Assert.NotNull(result?.Value);

        var accounts = result.Value.ToList();
        var returnedAccount = accounts.FirstOrDefault(a => a.Id == accountId);

        Assert.NotNull(returnedAccount);
        Assert.Equal(accountId, returnedAccount.Id);
        Assert.Equal("Test Account", returnedAccount.Name);
        Assert.Equal(0, returnedAccount.Balance.Amount);
        Assert.Equal("BRL", returnedAccount.Balance.Currency);
    }
}
