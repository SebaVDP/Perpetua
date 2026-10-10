namespace Perpetua.Structurizr.Domain;

/// <summary>
/// The outcome of an operation that can fail on user-supplied input: either a value or a failure.
/// It is exactly one of the nested cases; the private constructor keeps the set closed.
/// </summary>
public abstract record Result<T>
{
    private Result()
    {
    }

    public TOut Match<TOut>(Func<T, TOut> onSuccess, Func<TOut> onFailure)
        => this is Success success ? onSuccess(success.Value) : onFailure();

    public Result<TOut> Map<TOut>(Func<T, TOut> map)
        => Match<Result<TOut>>(value => new Result<TOut>.Success(map(value)), () => new Result<TOut>.Failure());

    public sealed record Success(T Value) : Result<T>;

    public sealed record Failure : Result<T>;
}
