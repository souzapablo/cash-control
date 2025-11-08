using CashControl.Domain.Primitives;

namespace CashControl.Domain.Errors;

public class CategoryErrors
{
    public static Error NotFound(Guid id) =>
        new("CATEGORY_NOT_FOUND", $"Category with ID '{id}' was not found.");
}
