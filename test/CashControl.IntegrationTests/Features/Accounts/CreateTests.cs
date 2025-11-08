using System.Net;
using System.Net.Http.Json;
using CashControl.Domain.Accounts;
using CashControl.Domain.Enums;
using CashControl.IntegrationTests.Extensions;
using CashControl.IntegrationTests.Infrastructure;
using CashControl.IntegrationTests.Models.Accounts;
using Xunit;
using static CashControl.Api.Feature.Accounts.Create;

namespace CashControl.IntegrationTests.Features.Accounts;

public class CreateTests : BaseIntegrationTest
{
    public CreateTests(IntegrationTestWebAppFactory factory)
        : base(factory) { }

    [Fact(DisplayName = "Should return 201 Created when account is created successfully")]
    public async Task Should_ReturnCreatedStatusCode_When_AccountIsCreated()
    {
        // Arrange
        Command command = new("My New Account");

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/accounts", command);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact(DisplayName = "Should return location header with account id when account is created")]
    public async Task Should_ReturnLocationHeader_When_AccountIsCreated()
    {
        // Arrange
        Command command = new("Account with Location Header");

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/accounts", command);

        // Assert
        Assert.NotNull(response.Headers.Location);

        var result = response.ReadAsResultAsync<CreateAccountResponse>();
        var location = response.Headers.Location.ToString();
        Assert.Contains(result!.Value.Id.ToString(), location);
    }

    [Fact(DisplayName = "Should persist account to database with correct name")]
    public async Task Should_PersistAccountToDatabase_When_AccountIsCreated()
    {
        // Arrange
        var accountName = "Persisted Account";
        Command command = new(accountName);

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/accounts", command);

        // Assert
        var result = response.ReadAsResultAsync<CreateAccountResponse>();
        Account? accountInDb = await GetAccountInDb(result?.Value.Id);

        Assert.NotNull(accountInDb);
        Assert.Equal(accountName, accountInDb.Name);
    }

    [Fact(DisplayName = "Should create account with maximum length name (200 characters)")]
    public async Task Should_CreateAccount_When_NameIsMaximumLength()
    {
        // Arrange
        var maxLengthName = new string('A', 200);
        Command command = new(maxLengthName);

        // Act
        var response = await Client.PostAsJsonAsync("/api/accounts", command);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = response.ReadAsResultAsync<CreateAccountResponse>();
        Account? accountInDb = await GetAccountInDb(result?.Value.Id);

        Assert.NotNull(accountInDb);
        Assert.Equal(maxLengthName, accountInDb.Name);
    }

    [Fact(DisplayName = "Should create account with special characters in name")]
    public async Task Should_CreateAccount_When_NameContainsSpecialCharacters()
    {
        // Arrange
        Command command = new("Account #1 - $pecial & Symbols! @2024");

        // Act
        var response = await Client.PostAsJsonAsync("/api/accounts", command);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = response.ReadAsResultAsync<CreateAccountResponse>();
        Account? accountInDb = await GetAccountInDb(result?.Value.Id);

        Assert.NotNull(accountInDb);
        Assert.Equal("Account #1 - $pecial & Symbols! @2024", accountInDb.Name);
    }

    [Fact(DisplayName = "Should create account with unicode characters in name")]
    public async Task Should_CreateAccount_When_NameContainsUnicodeCharacters()
    {
        // Arrange
        Command command = new("Conta Teste - 测试账户 - Тестовий рахунок");

        // Act
        var response = await Client.PostAsJsonAsync("/api/accounts", command);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = response.ReadAsResultAsync<CreateAccountResponse>();
        Account? accountInDb = await GetAccountInDb(result?.Value.Id);

        Assert.NotNull(accountInDb);
        Assert.Equal("Conta Teste - 测试账户 - Тестовий рахунок", accountInDb.Name);
    }

    [Fact(DisplayName = "Should return 400 Bad Request when name exceeds maximum length")]
    public async Task Should_ReturnBadRequest_When_NameExceedsMaximumLength()
    {
        // Arrange
        var tooLongName = new string('A', 201);
        Command command = new(tooLongName);

        // Act
        var response = await Client.PostAsJsonAsync("/api/accounts", command);

        // Assert
        var result = response.ReadAsResultAsync<CreateAccountResponse>();
        Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        Assert.Equal(
            Domain.Primitives.Error.ValidationError(
                "The field Name must be a string with a maximum length of '200'."
            ),
            result?.Error
        );
    }

    [Fact(DisplayName = "Should return 400 Bad Request when name is null")]
    public async Task Should_ReturnBadRequest_When_NameIsNull()
    {
        // Arrange
        Command command = new(null!);

        // Act
        var response = await Client.PostAsJsonAsync("/api/accounts", command);

        // Assert
        var result = response.ReadAsResultAsync<CreateAccountResponse>();
        Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        Assert.Equal(
            Domain.Primitives.Error.ValidationError("The field Name must be informed."),
            result?.Error
        );
    }

    [Fact(DisplayName = "Should return 400 Bad Request when name is empty string")]
    public async Task Should_ReturnBadRequest_When_NameIsEmpty()
    {
        // Arrange
        Command command = new(string.Empty);

        // Act
        var response = await Client.PostAsJsonAsync("/api/accounts", command);

        // Assert
        var result = response.ReadAsResultAsync<CreateAccountResponse>();
        Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        Assert.Equal(
            Domain.Primitives.Error.ValidationError("The field Name must be informed."),
            result?.Error
        );
    }

    [Fact(DisplayName = "Should create account with default currency BRL when currency is not provided")]
    public async Task Should_CreateAccount_WithDefaultCurrencyBRL_When_CurrencyNotProvided()
    {
        // Arrange
        Command command = new("Account with Default Currency");

        // Act
        var response = await Client.PostAsJsonAsync("/api/accounts", command);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = response.ReadAsResultAsync<CreateAccountResponse>();
        Account? accountInDb = await GetAccountInDb(result?.Value.Id);

        Assert.NotNull(accountInDb);
        Assert.Equal(Currency.BRL, accountInDb.Currency);
    }

    [Fact(DisplayName = "Should create account with USD currency when specified")]
    public async Task Should_CreateAccount_WithUSDCurrency_When_Specified()
    {
        // Arrange
        Command command = new("USD Account", Currency.USD);

        // Act
        var response = await Client.PostAsJsonAsync("/api/accounts", command);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = response.ReadAsResultAsync<CreateAccountResponse>();
        Account? accountInDb = await GetAccountInDb(result?.Value.Id);

        Assert.NotNull(accountInDb);
        Assert.Equal(Currency.USD, accountInDb.Currency);
    }

    [Fact(DisplayName = "Should return 400 Bad Request when currency is invalid enum value")]
    public async Task Should_ReturnBadRequest_When_CurrencyIsInvalidEnumValue()
    {
        // Arrange
        var invalidCommand = new { Name = "Test Account", Currency = 999 };

        // Act
        var response = await Client.PostAsJsonAsync("/api/accounts", invalidCommand);

        // Assert
        var result = response.ReadAsResultAsync<CreateAccountResponse>();
        Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        Assert.Equal(
            Domain.Primitives.Error.ValidationError("The field Currency must be a valid currency."),
            result?.Error
        );
    }

    private async Task<Account?> GetAccountInDb(Guid? id)
    {
        AccountId accountId = new(id.GetValueOrDefault());
        Account? accountInDb = await Context.Accounts.FindAsync(accountId);
        return accountInDb;
    }
}
