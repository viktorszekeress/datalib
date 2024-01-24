using System.Diagnostics.CodeAnalysis;

namespace Datalib.WebApi.Domain.Models;

public class Result
{
    protected object? ValueInternal { get; init; }

    public string? Error { get; protected init; }

    protected Result()
    {
    }

    public bool CheckSuccess()
    {
        return Error is null;
    }

    public static Result Ok() => new();

    public static Result<T> Ok<T>(T value) where T : class => new(value, null);

    public static Result Fail(string error) => new() {Error = error};

    public static Result<T> Fail<T>(string error) where T : class => new(default, error);
}

public class Result<T> : Result where T : class // to avoid boxing
{
    public T? Value => ValueInternal as T ?? default;

    public Result(T? value, string? error) => (ValueInternal, Error) = (value, error);
    
    public bool CheckSuccess([NotNullWhen(true)] out T? value)
    {
        value = Value;

        return CheckSuccess();
    }
}