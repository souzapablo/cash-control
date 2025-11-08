using CashControl.Domain.Primitives;

namespace CashControl.Domain.Errors;

public class TransactionErrors
{
    public static Error CurrencyMismatch =>
        new(
            "TRANSACTION_CURRENCY_MISMATCH",
            $"The transaction currency must match the account currency."
        );
}
