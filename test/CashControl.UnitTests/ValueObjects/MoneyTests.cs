using CashControl.Domain.Enums;
using CashControl.Domain.ValueObjects;
using Xunit;

namespace CashControl.UnitTests.ValueObjects;

public class MoneyTests
{
    [Fact(DisplayName = "Should create Money with correct value and currency")]
    public void Should_CreateMoney_WithCorrectValueAndCurrency()
    {
        // Arrange
        decimal value = 100.00m;
        Currency currency = Currency.BRL;

        // Act
        Money money = Money.Create(value, currency);

        // Assert
        Assert.Equal(value, money.Value);
        Assert.Equal(currency, money.Currency);
    }

    [Fact(DisplayName = "Zero should create Money with 0 value and specified currency")]
    public void Zero_Should_CreateMoney_WithZeroValueAndSpecifiedCurrency()
    {
        // Arrange
        decimal value = 0.00m;
        Currency currency = Currency.USD;

        // Act
        Money money = Money.Create(value, currency);

        // Assert
        Assert.Equal(0m, money.Value);
        Assert.Equal(currency, money.Currency);
    }

    [Fact(DisplayName = "Add should sum two Money values with same currency")]
    public void Add_Should_SumTwoMoneyValues_When_SameCurrency()
    {
        // Arrange
        decimal value1 = 50.00m;
        decimal value2 = 25.00m;
        Currency currency = Currency.BRL;

        Money money1 = Money.Create(value1, currency);
        Money money2 = Money.Create(value2, currency);

        // Act
        Money result = money1.Add(money2);

        // Assert
        Assert.Equal(value1 + value2, result.Value);
        Assert.Equal(currency, result.Currency);
    }

    [Fact(DisplayName = "Add should not sum two Money values with different currency")]
    public void Add_Should_NotSumTwoMoneyValues_When_DifferentCurrency()
    {
        // Arrange
        decimal value1 = 50.00m;
        decimal value2 = 25.00m;

        Money money1 = Money.Create(value1, Currency.BRL);
        Money money2 = Money.Create(value2, Currency.USD);

        // Act & Assert
        Assert
            .Throws<InvalidOperationException>(() => money1.Add(money2))
            .Message.Equals("Cannot add money with different currencies");
    }

    [Fact(DisplayName = "Subtract should subtract two Money values with same currency")]
    public void Subtract_Should_SubtractTwoMoneyValues_When_SameCurrency()
    {
        // Arrange
        decimal value1 = 50.00m;
        decimal value2 = 25.00m;
        Currency currency = Currency.BRL;

        Money money1 = Money.Create(value1, currency);
        Money money2 = Money.Create(value2, currency);

        // Act
        Money result = money1.Subtract(money2);

        // Assert
        Assert.Equal(value1 - value2, result.Value);
        Assert.Equal(currency, result.Currency);
    }

    [Fact(DisplayName = "Subtract should not sum two Money values with different currency")]
    public void Subtract_Should_NotSubtracyTwoMoneyValues_When_DifferentCurrency()
    {
        // Arrange
        decimal value1 = 50.00m;
        decimal value2 = 25.00m;

        Money money1 = Money.Create(value1, Currency.BRL);
        Money money2 = Money.Create(value2, Currency.USD);

        // Act & Assert
        Assert
            .Throws<InvalidOperationException>(() => money1.Subtract(money2))
            .Message.Equals("Cannot subtract money with different currencies");
    }

    [Fact(DisplayName = "Multiply should create Money with correct value and currency")]
    public void Multiply_Should_CreateMoneyWithCorrectValueAndCurrency()
    {
        // Arrange
        decimal value = 50.00m;
        Currency currency = Currency.BRL;

        Money money = Money.Create(value, currency);

        // Act
        Money result = money.Multiply(3);

        // Assert
        Assert.Equal(value * 3, result.Value);
        Assert.Equal(currency, result.Currency);
    }

    [Fact(DisplayName = "Negate should create Money with correct value and currency")]
    public void Negate_Should_CreateMoneyWithCorrectValueAndCurrency()
    {
        // Arrange
        decimal value = 50.00m;
        Currency currency = Currency.BRL;

        Money money = Money.Create(value, currency);

        // Act
        Money result = money.Negate();

        // Assert
        Assert.Equal(-value, result.Value);
        Assert.Equal(currency, result.Currency);
    }

    [Fact(
        DisplayName = "IsGreaterThan should return true for greater Money values with same currency"
    )]
    public void IsGreaterThan_Should_ReturnTrue_When_GreaterValueAndSameCurrency()
    {
        // Arrange
        decimal value1 = 50.00m;
        decimal value2 = 25.00m;
        Currency currency = Currency.BRL;

        Money money1 = Money.Create(value1, currency);
        Money money2 = Money.Create(value2, currency);

        // Act
        bool result = money1.IsGreaterThan(money2);

        // Assert
        Assert.True(result);
    }

    [Fact(
        DisplayName = "IsGreaterThan should return false for lesser Money values with same currency"
    )]
    public void IsGreaterThan_Should_ReturnFalse_When_LesserValueAndSameCurrency()
    {
        // Arrange
        decimal value1 = 50.00m;
        decimal value2 = 25.00m;
        Currency currency = Currency.BRL;

        Money money1 = Money.Create(value1, currency);
        Money money2 = Money.Create(value2, currency);

        // Act
        bool result = money2.IsGreaterThan(money1);

        // Assert
        Assert.False(result);
    }

    [Fact(DisplayName = "IsGreaterThan should not compare Money values with different currency")]
    public void IsGreaterThan_Should_ThrowInvalidOperationException_When_DifferentCurrency()
    {
        // Arrange
        decimal value1 = 50.00m;
        decimal value2 = 25.00m;

        Money money1 = Money.Create(value1, Currency.BRL);
        Money money2 = Money.Create(value2, Currency.USD);

        // Act & Assert
        Assert
            .Throws<InvalidOperationException>(() => money1.IsGreaterThan(money2))
            .Message.Equals("Cannot compare money with different currencies");
    }

    [Fact(DisplayName = "IsLessThan should return true for lesser Money values with same currency")]
    public void IsLessThan_Should_ReturnTrue_When_LesserValueAndSameCurrency()
    {
        // Arrange
        decimal value1 = 50.00m;
        decimal value2 = 25.00m;
        Currency currency = Currency.BRL;

        Money money1 = Money.Create(value1, currency);
        Money money2 = Money.Create(value2, currency);

        // Act
        bool result = money2.IsLessThan(money1);

        // Assert
        Assert.True(result);
    }

    [Fact(
        DisplayName = "IsLessThan should return false for greater Money values with same currency"
    )]
    public void IsLessThan_Should_ReturnFalse_When_GreaterValueAndSameCurrency()
    {
        // Arrange
        decimal value1 = 50.00m;
        decimal value2 = 25.00m;
        Currency currency = Currency.BRL;

        Money money1 = Money.Create(value1, currency);
        Money money2 = Money.Create(value2, currency);

        // Act
        bool result = money1.IsLessThan(money2);

        // Assert
        Assert.False(result);
    }

    [Fact(DisplayName = "IsLessThan should not compare Money values with different currency")]
    public void IsLessThan_Should_ThrowInvalidOperationException_When_DifferentCurrency()
    {
        // Arrange
        decimal value1 = 50.00m;
        decimal value2 = 25.00m;

        Money money1 = Money.Create(value1, Currency.BRL);
        Money money2 = Money.Create(value2, Currency.USD);

        // Act & Assert
        Assert
            .Throws<InvalidOperationException>(() => money1.IsLessThan(money2))
            .Message.Equals("Cannot compare money with different currencies");
    }

    [Fact(DisplayName = "Equals should return true for equal Money values with same currency")]
    public void IsEqualTo_Should_ReturnTrue_When_EqualValueAndSameCurrency()
    {
        // Arrange
        decimal value1 = 50.00m;
        decimal value2 = 50.00m;
        Currency currency = Currency.BRL;

        Money money1 = Money.Create(value1, currency);
        Money money2 = Money.Create(value2, currency);

        // Act
        bool result = money1.IsEqualTo(money2);

        // Assert
        Assert.True(result);
    }

    [Theory(
        DisplayName = "Equals should return false for greather or lesser Money values with same currency"
    )]
    [InlineData(39.999999, 40.00)]
    [InlineData(40.00, 39.999999)]
    public void IsEqualTo_Should_ReturnFalse_When_GreaterOrLesserValueAndSameCurrency(
        decimal value1,
        decimal value2
    )
    {
        // Arrange
        Currency currency = Currency.BRL;

        Money money1 = Money.Create(value1, currency);
        Money money2 = Money.Create(value2, currency);

        // Act
        bool result = money1.IsEqualTo(money2);

        // Assert
        Assert.False(result);
    }

    [Fact(DisplayName = "IsEqualTo should not compare Money values with different currency")]
    public void IsEqualTo_Should_ThrowInvalidOperationException_When_DifferentCurrency()
    {
        // Arrange
        decimal value1 = 50.00m;
        decimal value2 = 25.00m;

        Money money1 = Money.Create(value1, Currency.BRL);
        Money money2 = Money.Create(value2, Currency.USD);

        // Act & Assert
        Assert
            .Throws<InvalidOperationException>(() => money1.IsEqualTo(money2))
            .Message.Equals("Cannot compare money with different currencies");
    }
}
