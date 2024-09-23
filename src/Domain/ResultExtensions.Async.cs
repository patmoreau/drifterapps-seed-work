namespace DrifterApps.Seeds.Domain;

public static partial class ResultExtensions
{
    public static async Task<Result> OnSuccess(this Task<Result> resultTask, Func<Task<Result>> next)
    {
        var result = await resultTask;
        return result.IsSuccess ? await next() : result;
    }

    public static async Task<Result<TOut>> OnSuccess<TOut>(this Task<Result> resultTask, Func<Task<Result<TOut>>> next)
    {
        var result = await resultTask;
        return result.IsSuccess ? await next() : Result<TOut>.Failure(result.Error);
    }

    public static async Task<Result> OnSuccess<TIn>(this Task<Result<TIn>> resultTask, Func<TIn, Task<Result>> next)
    {
        var result = await resultTask;
        return result.IsSuccess ? await next(result.Value) : Result.Failure(result.Error);
    }

    public static async Task<Result<TOut>> OnSuccess<TIn, TOut>(this Task<Result<TIn>> resultTask,
        Func<TIn, Task<Result<TOut>>> next)
    {
        var result = await resultTask;
        return result.IsSuccess ? await next(result.Value) : Result<TOut>.Failure(result.Error);
    }

    public static async Task<Result> OnFailure(this Task<Result> resultTask, Func<ResultError, Task<Result>> next)
    {
        var result = await resultTask;
        return result.IsFailure ? await next(result.Error) : result;
    }

    public static async Task<Result> OnFailure<TIn>(this Task<Result<TIn>> resultTask,
        Func<ResultError, Task<Result>> next)
    {
        var result = await resultTask;
        return result.IsFailure ? await next(result.Error) : result;
    }

    public static async Task<Result> Match(this Task<Result> resultTask, Func<Task<Result>> onSuccess,
        Func<ResultError, Task<Result>> onFailure)
    {
        var result = await resultTask;
        return result.IsSuccess ? await onSuccess() : await onFailure(result.Error);
    }

    public static async Task<Result> Match<TIn>(this Task<Result<TIn>> resultTask, Func<TIn, Task<Result>> onSuccess,
        Func<ResultError, Task<Result>> onFailure)
    {
        var result = await resultTask;
        return result.IsSuccess ? await onSuccess(result.Value) : await onFailure(result.Error);
    }

    public static async Task<Result<TOut>> Match<TOut>(this Task<Result> resultTask, Func<Task<Result<TOut>>> onSuccess,
        Func<ResultError, Task<Result<TOut>>> onFailure)
    {
        var result = await resultTask;
        return result.IsSuccess ? await onSuccess() : await onFailure(result.Error);
    }

    public static async Task<Result<TOut>> Match<TIn, TOut>(this Task<Result<TIn>> resultTask,
        Func<TIn, Task<Result<TOut>>> onSuccess,
        Func<ResultError, Task<Result<TOut>>> onFailure)
    {
        var result = await resultTask;
        return result.IsSuccess ? await onSuccess(result.Value) : await onFailure(result.Error);
    }
}
