namespace Core.Models;

public abstract record Result<T>;

public sealed record Success<T>(T Value) : Result<T>;
public sealed record Error<T>(string Message) : Result<T>;

public static class ResultExtensions
{
    public static Result<U> Bind<T, U>(this Result<T> result, Func<T, Result<U>> binder)
    {
        return result switch
        {
            Success<T> success => binder(success.Value),
            Error<T> error => new Error<U>(error.Message),
            _ => throw new InvalidOperationException($"Ukjent resultattype: {result?.GetType().Name}")
        };
    }
}
