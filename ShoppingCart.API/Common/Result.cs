namespace ShoppingCart.Api.Common;

public record Error(string Code, string Message);

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Data { get; }
    public Error? Error { get; }

    private Result(bool isSuccess, T? data, Error? error)
    {
        IsSuccess = isSuccess;
        Data = data;
        Error = error;
    }

    public static Result<T> Success(T data)
        => new(true, data, null);

    public static Result<T> Failure(string code, string message)
        => new(false, default, new Error(code, message));
}
