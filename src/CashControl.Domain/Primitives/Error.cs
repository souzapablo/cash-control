namespace CashControl.Domain.Primitives;

public record Error(string Code, string Message)
{
    public static Error ValidationError(string message) => new("VALIDATION_ERROR", message);
};
