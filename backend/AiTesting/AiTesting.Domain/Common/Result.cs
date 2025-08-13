namespace AiTesting.Domain.Common;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }

    protected Result(bool isSuccess, string error)
    {
        if (isSuccess && !string.IsNullOrWhiteSpace(error))
            throw new InvalidOperationException("Successful result cannot have an error message.");
        if (!isSuccess && string.IsNullOrWhiteSpace(error))
            throw new InvalidOperationException("Failure result must have an error message.");

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new Result(true, null);
    public static Result Failure(string error) => new Result(false, error);
    public Result<T> ToResult<T>(T? value = default) =>
        IsSuccess ? Result<T>.Success(value!) : Result<T>.Failure(Error);
}

public class Result<T> : Result
{
    private readonly T _value;

    public T Value
    {
        get
        {
            if (!IsSuccess)
                throw new InvalidOperationException("Cannot access the value of a failed result.");
            return _value;
        }
    }

    protected internal Result(T value, bool isSuccess, string error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public static Result<T> Success(T value) => new (value, true, string.Empty);
    public static new Result<T> Failure(string error) => new (default, false, error);

    public Result<TResult> Map<TResult>(Func<T, TResult> func) =>
        IsSuccess ? Result<TResult>.Success(func(_value)) : Result<TResult>.Failure(Error);

    public Result<TResult> Bind<TResult>(Func<T, Result<TResult>> func) =>
        IsSuccess ? func(_value) : Result<TResult>.Failure(Error);
}
