using System.Net;
using System.Net.Http.Json;
using CashControl.Domain.Accounts;
using CashControl.Domain.Enums;
using CashControl.Domain.Errors;
using CashControl.Domain.Transactions;
using CashControl.IntegrationTests.Extensions;
using CashControl.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;
using static CashControl.Api.Feature.Accounts.CreateTransaction;

namespace CashControl.IntegrationTests.Features.Accounts;

public class CreateTransactionTests : BaseIntegrationTest
{
    public CreateTransactionTests(IntegrationTestWebAppFactory factory)
        : base(factory) { }

    [Fact(DisplayName = "Should return 201 Created when transaction is created successfully")]
    public async Task Should_ReturnCreatedStatusCode_When_TransactionIsCreated()
    {
        // Arrange
        Command command = new(
            Data.DefaultCategory.Id.Value,
            "Test Transaction",
            100.50m,
            Currency.BRL,
            DateTime.UtcNow,
            TransactionType.Income
        );

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync(
            $"/api/accounts/{Data.DefaultAccount.Id.Value}/transactions",
            command
        );

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact(
        DisplayName = "Should return location header with transaction id when transaction is created"
    )]
    public async Task Should_ReturnLocationHeader_When_TransactionIsCreated()
    {
        // Arrange
        Command command = new(
            Data.DefaultCategory.Id.Value,
            "Transaction with Location Header",
            50.25m,
            Currency.BRL,
            DateTime.UtcNow,
            TransactionType.Income
        );

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync(
            $"/api/accounts/{Data.DefaultAccount.Id.Value}/transactions",
            command
        );

        // Assert
        Assert.NotNull(response.Headers.Location);

        var result = response.ReadAsResultAsync<Response>();
        var location = response.Headers.Location.ToString();
        Assert.Contains(result!.Value.Id.ToString(), location);
        Assert.Contains($"accounts/{Data.DefaultAccount.Id.Value}/transactions", location);
    }

    [Fact(DisplayName = "Should persist transaction to database with correct values")]
    public async Task Should_PersistTransactionToDatabase_When_TransactionIsCreated()
    {
        // Arrange
        var description = "Persisted Transaction";
        var amount = 75.99m;
        var currency = Currency.BRL;
        var date = DateTime.UtcNow;
        Command command = new(
            Data.DefaultCategory.Id.Value,
            description,
            amount,
            currency,
            date,
            TransactionType.Income
        );

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync(
            $"/api/accounts/{Data.DefaultAccount.Id.Value}/transactions",
            command
        );

        // Assert
        var result = response.ReadAsResultAsync<Response>();
        Transaction? transactionInDb = await GetTransactionInDb(result?.Value.Id);

        Assert.NotNull(transactionInDb);
        Assert.Equal(description, transactionInDb.Description);
        Assert.Equal(amount, transactionInDb.Amount.Value);
        Assert.Equal(currency, transactionInDb.Amount.Currency);
        Assert.Equal(date, transactionInDb.Date, TimeSpan.FromSeconds(1));
        Assert.Equal(TransactionType.Income, transactionInDb.Type);
        Assert.Equal(Data.DefaultAccount.Id.Value, transactionInDb.AccountId.Value);
    }

    [Fact(DisplayName = "Should update account balance when income transaction is created")]
    public async Task Should_UpdateAccountBalance_When_IncomeTransactionIsCreated()
    {
        // Arrange
        Account? account = await Context.Accounts.SingleOrDefaultAsync(a =>
            a.Id == Data.DefaultAccount.Id
        );
        var initialBalance = account?.Balance.Value;
        var transactionAmount = 150.75m;
        Command command = new(
            Data.DefaultCategory.Id.Value,
            "Income Transaction",
            transactionAmount,
            Currency.BRL,
            DateTime.UtcNow,
            TransactionType.Income
        );

        // Act
        await Client.PostAsJsonAsync(
            $"/api/accounts/{Data.DefaultAccount.Id.Value}/transactions",
            command
        );

        // Assert
        Account? accountInDb = await GetAccountInDb(Data.DefaultAccount.Id.Value);
        Assert.NotNull(accountInDb);
        Assert.Equal(initialBalance + transactionAmount, accountInDb.Balance.Value);
    }

    [Fact(DisplayName = "Should update account balance when expense transaction is created")]
    public async Task Should_UpdateAccountBalance_When_ExpenseTransactionIsCreated()
    {
        // Arrange
        Account? account = await GetAccountInDb(Data.DefaultAccount.Id.Value)!;
        var balanceBeforeExpense = account?.Balance.Value;

        var expenseAmount = 50.25m;

        var expenseCommand = new Command(
            Data.DefaultCategory.Id.Value,
            "Expense Transaction",
            expenseAmount,
            Currency.BRL,
            DateTime.UtcNow,
            TransactionType.Expense
        );

        // Act
        await Client.PostAsJsonAsync(
            $"/api/accounts/{account?.Id.Value}/transactions",
            expenseCommand
        );

        // Assert
        var expectedBalance = balanceBeforeExpense - expenseAmount;
        Account? accountInDb = await GetAccountInDb(account!.Id.Value);
        Assert.NotNull(accountInDb);
        Assert.Equal(expectedBalance, accountInDb.Balance.Value);
    }

    [Fact(DisplayName = "Should accumulate balance correctly with multiple transactions")]
    public async Task Should_AccumulateBalanceCorrectly_WithMultipleTransactions()
    {
        // Arrange
        Account? account = await GetAccountInDb(Data.DefaultAccount.Id.Value);
        var initialBalance = account?.Balance.Value;
        var initialTransactionsCount = account?.Transactions.Count ?? 0;

        var transactions = new[]
        {
            new Command(
                Data.DefaultCategory.Id.Value,
                "Income 1",
                100m,
                Currency.BRL,
                DateTime.UtcNow,
                TransactionType.Income
            ),
            new Command(
                Data.DefaultCategory.Id.Value,
                "Income 2",
                50m,
                Currency.BRL,
                DateTime.UtcNow,
                TransactionType.Income
            ),
            new Command(
                Data.DefaultCategory.Id.Value,
                "Expense 1",
                30m,
                Currency.BRL,
                DateTime.UtcNow,
                TransactionType.Expense
            ),
            new Command(
                Data.DefaultCategory.Id.Value,
                "Income 3",
                25m,
                Currency.BRL,
                DateTime.UtcNow,
                TransactionType.Income
            ),
            new Command(
                Data.DefaultCategory.Id.Value,
                "Expense 2",
                15m,
                Currency.BRL,
                DateTime.UtcNow,
                TransactionType.Expense
            ),
        };

        // Act
        foreach (var transaction in transactions)
        {
            await Client.PostAsJsonAsync(
                $"/api/accounts/{account?.Id.Value}/transactions",
                transaction
            );
        }

        // Assert
        Account? accountInDb = await GetAccountInDb(account!.Id.Value);
        Assert.NotNull(accountInDb);
        var expectedBalance = initialBalance + 100m + 50m - 30m + 25m - 15m;
        var expectedTransactionsCount = initialTransactionsCount + transactions.Length;
        Assert.Equal(expectedBalance, accountInDb.Balance.Value);
        Assert.Equal(expectedTransactionsCount, accountInDb.Transactions.Count);
    }

    [Fact(DisplayName = "Should return 404 Not Found when account does not exist")]
    public async Task Should_ReturnNotFound_When_AccountDoesNotExist()
    {
        // Arrange
        Guid nonExistentAccountId = Guid.NewGuid();
        Command command = new(
            Data.DefaultCategory.Id.Value,
            "Test Transaction",
            100m,
            Currency.BRL,
            DateTime.UtcNow,
            TransactionType.Income
        );

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync(
            $"/api/accounts/{nonExistentAccountId}/transactions",
            command
        );

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName = "Should return AccountNotFound error when account does not exist")]
    public async Task Should_ReturnAccountNotFoundError_When_AccountDoesNotExist()
    {
        // Arrange
        Guid nonExistentAccountId = Guid.NewGuid();
        Command command = new(
            Data.DefaultCategory.Id.Value,
            "Test Transaction",
            100m,
            Currency.BRL,
            DateTime.UtcNow,
            TransactionType.Income
        );

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync(
            $"/api/accounts/{nonExistentAccountId}/transactions",
            command
        );
        var result = response.ReadAsResultAsync<Response>();

        // Assert
        Assert.False(result?.IsSuccess);
        Assert.NotNull(result?.Error);
        Assert.Equal("ACCOUNT_NOT_FOUND", result.Error.Code);
        Assert.Contains(nonExistentAccountId.ToString(), result.Error.Message);
    }

    [Fact(DisplayName = "Should return 400 Bad Request when amount is zero")]
    public async Task Should_ReturnBadRequest_When_AmountIsZero()
    {
        // Arrange
        Command command = new(
            Data.DefaultCategory.Id.Value,
            "Zero Amount Transaction",
            0m,
            Currency.BRL,
            DateTime.UtcNow,
            TransactionType.Income
        );

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync(
            $"/api/accounts/{Data.DefaultAccount.Id.Value}/transactions",
            command
        );

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var result = response.ReadAsResultAsync<Response>();
        Assert.Equal(
            Domain.Primitives.Error.ValidationError("The field Amount must be greater than zero."),
            result?.Error
        );
    }

    [Fact(DisplayName = "Should return 400 Bad Request when amount is negative")]
    public async Task Should_ReturnBadRequest_When_AmountIsNegative()
    {
        // Arrange
        Command command = new(
            Data.DefaultCategory.Id.Value,
            "Negative Amount Transaction",
            -50m,
            Currency.BRL,
            DateTime.UtcNow,
            TransactionType.Income
        );

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync(
            $"/api/accounts/{Data.DefaultAccount.Id.Value}/transactions",
            command
        );

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var result = response.ReadAsResultAsync<Response>();
        Assert.Equal(
            Domain.Primitives.Error.ValidationError("The field Amount must be greater than zero."),
            result?.Error
        );
    }

    [Fact(DisplayName = "Should create transaction with large amount")]
    public async Task Should_CreateTransaction_When_AmountIsLarge()
    {
        // Arrange
        var largeAmount = 999999999.9999m;
        Command command = new(
            Data.DefaultCategory.Id.Value,
            "Large Amount Transaction",
            largeAmount,
            Currency.BRL,
            DateTime.UtcNow,
            TransactionType.Income
        );

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync(
            $"/api/accounts/{Data.DefaultAccount.Id.Value}/transactions",
            command
        );

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = response.ReadAsResultAsync<Response>();
        Transaction? transactionInDb = await GetTransactionInDb(result?.Value.Id);
        Assert.NotNull(transactionInDb);
        Assert.Equal(largeAmount, transactionInDb.Amount.Value);
    }

    [Fact(DisplayName = "Should not create transaction with different currencies than account")]
    public async Task Should_NotCreateTransaction_WithDifferentCurrencies()
    {
        // Arrange
        var usdCommand = new Command(
            Data.DefaultCategory.Id.Value,
            "USD Transaction",
            100m,
            Currency.USD,
            DateTime.UtcNow,
            TransactionType.Income
        );

        // Act
        var usdResponse = await Client.PostAsJsonAsync(
            $"/api/accounts/{Data.DefaultAccount.Id.Value}/transactions",
            usdCommand
        );

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, usdResponse.StatusCode);

        var usdResult = usdResponse.ReadAsResultAsync<Response>();

        Assert.Equal(TransactionErrors.CurrencyMismatch, usdResult?.Error);
    }

    [Fact(
        DisplayName = "Should create transaction with maximum length description (200 characters)"
    )]
    public async Task Should_CreateTransaction_When_DescriptionIsMaximumLength()
    {
        // Arrange
        var maxLengthDescription = new string('A', 200);
        Command command = new(
            Data.DefaultCategory.Id.Value,
            maxLengthDescription,
            100m,
            Currency.BRL,
            DateTime.UtcNow,
            TransactionType.Income
        );

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync(
            $"/api/accounts/{Data.DefaultAccount.Id.Value}/transactions",
            command
        );

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = response.ReadAsResultAsync<Response>();
        Transaction? transactionInDb = await GetTransactionInDb(result?.Value.Id);
        Assert.NotNull(transactionInDb);
        Assert.Equal(maxLengthDescription, transactionInDb.Description);
    }

    [Fact(DisplayName = "Should create transaction with special characters in description")]
    public async Task Should_CreateTransaction_When_DescriptionContainsSpecialCharacters()
    {
        // Arrange
        var description = "Transaction #1 - $pecial & Symbols! @2024";
        Command command = new(
            Data.DefaultCategory.Id.Value,
            description,
            100m,
            Currency.BRL,
            DateTime.UtcNow,
            TransactionType.Income
        );

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync(
            $"/api/accounts/{Data.DefaultAccount.Id.Value}/transactions",
            command
        );

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = response.ReadAsResultAsync<Response>();
        Transaction? transactionInDb = await GetTransactionInDb(result?.Value.Id);
        Assert.NotNull(transactionInDb);
        Assert.Equal(description, transactionInDb.Description);
    }

    [Fact(DisplayName = "Should create transaction with unicode characters in description")]
    public async Task Should_CreateTransaction_When_DescriptionContainsUnicodeCharacters()
    {
        // Arrange
        var description = "Transação Teste - 测试交易 - Тестова транзакція";
        Command command = new(
            Data.DefaultCategory.Id.Value,
            description,
            100m,
            Currency.BRL,
            DateTime.UtcNow,
            TransactionType.Income
        );

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync(
            $"/api/accounts/{Data.DefaultAccount.Id.Value}/transactions",
            command
        );

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = response.ReadAsResultAsync<Response>();
        Transaction? transactionInDb = await GetTransactionInDb(result?.Value.Id);
        Assert.NotNull(transactionInDb);
        Assert.Equal(description, transactionInDb.Description);
    }

    [Fact(DisplayName = "Should create transaction with past date")]
    public async Task Should_CreateTransaction_WithPastDate()
    {
        // Arrange
        var pastDate = DateTime.UtcNow.AddDays(-30);
        Command command = new(
            Data.DefaultCategory.Id.Value,
            "Past Transaction",
            100m,
            Currency.BRL,
            pastDate,
            TransactionType.Income
        );

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync(
            $"/api/accounts/{Data.DefaultAccount.Id.Value}/transactions",
            command
        );

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = response.ReadAsResultAsync<Response>();
        Transaction? transactionInDb = await GetTransactionInDb(result?.Value.Id);
        Assert.NotNull(transactionInDb);
        Assert.Equal(pastDate, transactionInDb.Date, TimeSpan.FromSeconds(1));
    }

    [Fact(DisplayName = "Should create transaction with future date")]
    public async Task Should_CreateTransaction_WithFutureDate()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddDays(30);
        Command command = new(
            Data.DefaultCategory.Id.Value,
            "Future Transaction",
            100m,
            Currency.BRL,
            futureDate,
            TransactionType.Income
        );

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync(
            $"/api/accounts/{Data.DefaultAccount.Id.Value}/transactions",
            command
        );

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = response.ReadAsResultAsync<Response>();
        Transaction? transactionInDb = await GetTransactionInDb(result?.Value.Id);
        Assert.NotNull(transactionInDb);
        Assert.Equal(futureDate, transactionInDb.Date, TimeSpan.FromSeconds(1));
    }

    [Fact(DisplayName = "Should return 400 Bad Request when description is null")]
    public async Task Should_ReturnBadRequest_When_DescriptionIsNull()
    {
        // Arrange
        Command command = new(
            Data.DefaultCategory.Id.Value,
            null!,
            100m,
            Currency.BRL,
            DateTime.UtcNow,
            TransactionType.Income
        );

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync(
            $"/api/accounts/{Data.DefaultAccount.Id.Value}/transactions",
            command
        );

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var result = response.ReadAsResultAsync<Response>();
        Assert.Equal(
            Domain.Primitives.Error.ValidationError("The field Description must be informed."),
            result?.Error
        );
    }

    [Fact(DisplayName = "Should return 400 Bad Request when description is empty string")]
    public async Task Should_ReturnBadRequest_When_DescriptionIsEmpty()
    {
        // Arrange
        Command command = new(
            Data.DefaultCategory.Id.Value,
            string.Empty,
            100m,
            Currency.BRL,
            DateTime.UtcNow,
            TransactionType.Income
        );

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync(
            $"/api/accounts/{Data.DefaultAccount.Id.Value}/transactions",
            command
        );

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var result = response.ReadAsResultAsync<Response>();
        Assert.Equal(
            Domain.Primitives.Error.ValidationError("The field Description must be informed."),
            result?.Error
        );
    }

    [Fact(DisplayName = "Should return 400 Bad Request when description exceeds maximum length")]
    public async Task Should_ReturnBadRequest_When_DescriptionExceedsMaximumLength()
    {
        // Arrange
        var tooLongDescription = new string('A', 201);
        Command command = new(
            Data.DefaultCategory.Id.Value,
            tooLongDescription,
            100m,
            Currency.BRL,
            DateTime.UtcNow,
            TransactionType.Income
        );

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync(
            $"/api/accounts/{Data.DefaultAccount.Id.Value}/transactions",
            command
        );

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var result = response.ReadAsResultAsync<Response>();
        Assert.Equal(
            Domain.Primitives.Error.ValidationError(
                "The field Description must be a string with a maximum length of '200'."
            ),
            result?.Error
        );
    }

    [Fact(DisplayName = "Should return 400 Bad Request when currency is invalid enum value")]
    public async Task Should_ReturnBadRequest_When_CurrencyIsInvalidEnumValue()
    {
        // Arrange
        var invalidCommand = new
        {
            Description = "Test Transaction",
            Amount = 100m,
            Currency = 999,
            Date = DateTime.UtcNow,
            Type = TransactionType.Income,
        };

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync(
            $"/api/accounts/{Data.DefaultAccount.Id.Value}/transactions",
            invalidCommand
        );

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var result = response.ReadAsResultAsync<Response>();
        Assert.Equal(
            Domain.Primitives.Error.ValidationError("The field Currency must be a valid currency."),
            result?.Error
        );
    }

    [Fact(
        DisplayName = "Should return 400 Bad Request when transaction type is invalid enum value"
    )]
    public async Task Should_ReturnBadRequest_When_TransactionTypeIsInvalidEnumValue()
    {
        // Arrange
        var invalidCommand = new
        {
            Description = "Test Transaction",
            Amount = 100m,
            Currency = Currency.BRL,
            Date = DateTime.UtcNow,
            Type = 999,
        };

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync(
            $"/api/accounts/{Data.DefaultAccount.Id.Value}/transactions",
            invalidCommand
        );

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var result = response.ReadAsResultAsync<Response>();
        Assert.Equal(
            Domain.Primitives.Error.ValidationError(
                "The field Type must be a valid transaction type."
            ),
            result?.Error
        );
    }

    [Fact(DisplayName = "Should return 400 Bad Request when category does not exist")]
    public async Task Should_ReturnBadRequest_When_CategoryDoesNotExist()
    {
        // Arrange
        Command command = new(
            Guid.NewGuid(),
            "Category does not exist",
            100m,
            Currency.BRL,
            DateTime.UtcNow,
            TransactionType.Income
        );

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync(
            $"/api/accounts/{Data.DefaultAccount.Id.Value}/transactions",
            command
        );

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var result = response.ReadAsResultAsync<Response>();
        Assert.Equal(CategoryErrors.NotFound(command.CategoryId), result?.Error);
    }

    private async Task<Account?> GetAccountInDb(Guid id)
    {
        AccountId accountId = AccountId.Create(id);
        Account? account = await Context
            .Accounts.Include(a => a.Transactions)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == accountId);
        return account;
    }

    private async Task<Transaction?> GetTransactionInDb(Guid? id)
    {
        if (id is null)
            return null;

        TransactionId transactionId = TransactionId.Create(id.Value);
        Transaction? transaction = await Context.Transactions.FirstOrDefaultAsync(t =>
            t.Id == transactionId
        );
        return transaction;
    }
}
