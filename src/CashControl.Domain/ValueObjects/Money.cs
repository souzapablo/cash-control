using CashControl.Domain.Enums;

namespace CashControl.Domain.ValueObjects;

public record Money
{
    private Money(decimal amount, Currency currency)
    {
        Amount = amount;
        Currency = currency;
    }
    
    public decimal Amount { get; init; }
    public Currency Currency { get; init; }

    public static Money Zero(Currency currency = Currency.BRL) 
        => new(0, currency);
    
    public static Money Create(decimal value, Currency currency = Currency.BRL)
        => new(value, currency);
    
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot add money with different currencies");
        
        return new Money(Amount + other.Amount, Currency);
    }
    
    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot subtract money with different currencies");
        
        return new Money(Amount - other.Amount, Currency);
    }
    
    public Money Multiply(decimal multiplier)
        => new(Amount * multiplier, Currency);
    
    public Money Negate()
        => new(-Amount, Currency);
    
    public bool IsGreaterThan(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot compare money with different currencies");
        
        return Amount > other.Amount;
    }

    public bool IsLessThan(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot compare money with different currencies");

        return Amount < other.Amount;
    }
    
    public bool IsEqualTo(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot compare money with different currencies");
        
        return Amount == other.Amount;
    }
    
    public bool IsZero => Amount == 0;
    public bool IsNegative => Amount < 0;
    public bool IsPositive => Amount > 0;
}