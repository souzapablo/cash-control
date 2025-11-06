using CashControl.Domain.ValueObjects;

namespace CashControl.Api.Responses.ValueObjects;

public class MoneyResponse(Money money)
{
    public decimal Amount { get; init; } = money.Amount;
    public string Currency { get; init; } = money.Currency.ToString();
}
