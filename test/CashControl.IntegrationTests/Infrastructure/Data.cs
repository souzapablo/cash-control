using CashControl.Domain.Accounts;
using CashControl.Domain.Categories;
using CashControl.Domain.Enums;
using CashControl.Domain.Transactions;
using CashControl.Domain.Users;
using CashControl.Domain.ValueObjects;
using PasswordHasher = BCrypt.Net.BCrypt;

namespace CashControl.IntegrationTests.Infrastructure;

public class Data
{
    public static readonly string DefaultPassword = "DefaultPassword123!";
    public static readonly Account DefaultAccount = Account.Create("Default Account", Currency.BRL);
    public static readonly Category DefaultCategory = Category.Create("Default");
    public static readonly Transaction DefaultTransaction = Transaction.Create(
        DefaultCategory.Id,
        "Default Transaction",
        Money.Create(100, Currency.BRL),
        TransactionType.Income,
        DateTime.UtcNow
    );
    public static readonly User DefaultUser = User.Create(
        Email.Create("defaultuser@example.com"),
        PasswordHasher.HashPassword(DefaultPassword)
    );
}
