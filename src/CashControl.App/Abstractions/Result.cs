namespace CashControl.App.Abstractions;

public class Result
{
    protected Result(bool isSuccess, Error? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public Error? Error { get; }
    public bool IsSuccess { get; }

    public static Result Success() => new(true, null);
    public static Result<T> Success<T>(T value) => new(value);

    public static Result Failure(Error error) => new(false, error);
    public static Result<T> Failure<T>(Error error) => new(error);
}

public class Result<TData> : Result
{
    internal Result(TData value)
        : base(true, null)
    {
        Value = value;
    }

    internal Result(Error error)
        : base(false, error)
    {
        Value = default;
    }
    public TData? Value { get; }
}
