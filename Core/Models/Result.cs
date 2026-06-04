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

    public static Result<BookingForm> BindValidRoomType(this Result<BookingForm> result, string? requestedRoomType)
    {
        return result is Success<BookingForm> success
            ? Enum.TryParse<RoomType>(requestedRoomType, true, out var roomType)
                ? new Success<BookingForm>(success.Value with { RoomType = roomType })
                : new Error<BookingForm>($"Romtype '{requestedRoomType}' er ikke gyldig.")
            : result;
    }
}
