using CashControl.Domain.Primitives;

namespace CashControl.IntegrationTests.Models;

public class Result<TData>
{
    public TData Value { get; set; } = default!;
    public Error? Error { get; set; } = null;
    public bool IsSuccess { get; set; }
}

public class Result
{
    public Error? Error { get; set; }
    public bool IsSuccess { get; set; }
}