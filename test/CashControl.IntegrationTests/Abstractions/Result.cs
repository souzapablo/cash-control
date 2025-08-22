using CashControl.App.Abstractions;

namespace CashControl.IntegrationTests.Abstractions;

public class Result
{
    public Error Error { get; set; } = null!;
    public bool IsSuccess { get; set; }
}

public class Result<T>
{
    public T Value { get; set; } = default!;
    public Error Error { get; set; } = null!;
    public bool IsSuccess { get; set; }
}