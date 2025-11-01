using CashControl.Domain.ValueObjects;

namespace CashControl.Api.Responses.ValueObjects;

public class MoneyResponse(Money money)
{
    public decimal Value { get; init; } = money.Value;
    public string Currency { get; init; } = money.Currency.ToString();
}
