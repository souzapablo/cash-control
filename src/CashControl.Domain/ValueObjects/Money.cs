using CashControl.Domain.Enums;

namespace CashControl.Domain.ValueObjects;

public record Money
{
    private Money(decimal value, Currency currency)
    {
        Value = value;
        Currency = currency;
    }
    
    public decimal Value { get; init; }
    public Currency Currency { get; init; }

    public static Money Zero(Currency currency = Currency.BRL) 
        => new(0, currency);
    
    public static Money Create(decimal value, Currency currency = Currency.BRL)
        => new(value, currency);
    
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot add money with different currencies");
        
        return new Money(Value + other.Value, Currency);
    }
    
    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot subtract money with different currencies");
        
        return new Money(Value - other.Value, Currency);
    }
    
    public Money Multiply(decimal multiplier)
        => new(Value * multiplier, Currency);
    
    public Money Negate()
        => new(-Value, Currency);
    
    public bool IsGreaterThan(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot compare money with different currencies");
        
        return Value > other.Value;
    }
    
    public bool IsLessThan(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot compare money with different currencies");
        
        return Value < other.Value;
    }
    
    public bool IsZero => Value == 0;
    public bool IsNegative => Value < 0;
    public bool IsPositive => Value > 0;
}