using CashControl.IntegrationTests.Models.ValueObjects;

namespace CashControl.IntegrationTests.Models.Accounts;

public record AccountDetailsResponse(Guid Id, string Name, MoneyResponse Balance);
