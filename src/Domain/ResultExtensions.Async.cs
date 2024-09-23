namespace DrifterApps.Seeds.Domain;

/// <summary>
///     Provides extension methods for handling results asynchronously.
/// </summary>
public static partial class ResultExtensions
{
    /// <summary>
    ///     Executes the next function if the result is successful.
    /// </summary>
    /// <param name="resultTask">The task that returns a result.</param>
    /// <param name="next">The function to execute if the result is successful.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task<Result> OnSuccess(this Task<Result> resultTask, Func<Task<Result>> next)
    {
        var result = await resultTask;
        return result.IsSuccess ? await next() : result;
    }

    /// <summary>
    ///     Executes the next function if the result is successful and returns a result of type TOut.
    /// </summary>
    /// <typeparam name="TOut">The type of the result returned by the next function.</typeparam>
    /// <param name="resultTask">The task that returns a result.</param>
    /// <param name="next">The function to execute if the result is successful.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task<Result<TOut>> OnSuccess<TOut>(this Task<Result> resultTask, Func<Task<Result<TOut>>> next)
    {
        var result = await resultTask;
        return result.IsSuccess ? await next() : Result<TOut>.Failure(result.Error);
    }

    /// <summary>
    ///     Executes the next function if the result is successful, passing the result value to the next function.
    /// </summary>
    /// <typeparam name="TIn">The type of the input result value.</typeparam>
    /// <param name="resultTask">The task that returns a result of type TIn.</param>
    /// <param name="next">The function to execute if the result is successful.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task<Result> OnSuccess<TIn>(this Task<Result<TIn>> resultTask, Func<TIn, Task<Result>> next)
    {
        var result = await resultTask;
        return result.IsSuccess ? await next(result.Value) : Result.Failure(result.Error);
    }

    /// <summary>
    ///     Executes the next function if the result is successful, passing the result value to the next function and returning
    ///     a result of type TOut.
    /// </summary>
    /// <typeparam name="TIn">The type of the input result value.</typeparam>
    /// <typeparam name="TOut">The type of the result returned by the next function.</typeparam>
    /// <param name="resultTask">The task that returns a result of type TIn.</param>
    /// <param name="next">The function to execute if the result is successful.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task<Result<TOut>> OnSuccess<TIn, TOut>(this Task<Result<TIn>> resultTask,
        Func<TIn, Task<Result<TOut>>> next)
    {
        var result = await resultTask;
        return result.IsSuccess ? await next(result.Value) : Result<TOut>.Failure(result.Error);
    }

    /// <summary>
    ///     Executes the next function if the result is a failure.
    /// </summary>
    /// <param name="resultTask">The task that returns a result.</param>
    /// <param name="next">The function to execute if the result is a failure.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task<Result> OnFailure(this Task<Result> resultTask, Func<ResultError, Task<Result>> next)
    {
        var result = await resultTask;
        return result.IsFailure ? await next(result.Error) : result;
    }

    /// <summary>
    ///     Executes the next function if the result is a failure, passing the error to the next function.
    /// </summary>
    /// <typeparam name="TIn">The type of the input result value.</typeparam>
    /// <param name="resultTask">The task that returns a result of type TIn.</param>
    /// <param name="next">The function to execute if the result is a failure.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task<Result> OnFailure<TIn>(this Task<Result<TIn>> resultTask,
        Func<ResultError, Task<Result>> next)
    {
        var result = await resultTask;
        return result.IsFailure ? await next(result.Error) : result;
    }

    /// <summary>
    ///     Matches the result to either the onSuccess or onFailure function.
    /// </summary>
    /// <param name="resultTask">The task that returns a result.</param>
    /// <param name="onSuccess">The function to execute if the result is successful.</param>
    /// <param name="onFailure">The function to execute if the result is a failure.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task<Result> Match(this Task<Result> resultTask, Func<Task<Result>> onSuccess,
        Func<ResultError, Task<Result>> onFailure)
    {
        var result = await resultTask;
        return result.IsSuccess ? await onSuccess() : await onFailure(result.Error);
    }

    /// <summary>
    ///     Matches the result to either the onSuccess or onFailure function, passing the result value to the onSuccess
    ///     function.
    /// </summary>
    /// <typeparam name="TIn">The type of the input result value.</typeparam>
    /// <param name="resultTask">The task that returns a result of type TIn.</param>
    /// <param name="onSuccess">The function to execute if the result is successful.</param>
    /// <param name="onFailure">The function to execute if the result is a failure.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task<Result> Match<TIn>(this Task<Result<TIn>> resultTask, Func<TIn, Task<Result>> onSuccess,
        Func<ResultError, Task<Result>> onFailure)
    {
        var result = await resultTask;
        return result.IsSuccess ? await onSuccess(result.Value) : await onFailure(result.Error);
    }

    /// <summary>
    ///     Matches the result to either the onSuccess or onFailure function, returning a result of type TOut.
    /// </summary>
    /// <typeparam name="TOut">The type of the result returned by the onSuccess or onFailure function.</typeparam>
    /// <param name="resultTask">The task that returns a result.</param>
    /// <param name="onSuccess">The function to execute if the result is successful.</param>
    /// <param name="onFailure">The function to execute if the result is a failure.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task<Result<TOut>> Match<TOut>(this Task<Result> resultTask, Func<Task<Result<TOut>>> onSuccess,
        Func<ResultError, Task<Result<TOut>>> onFailure)
    {
        var result = await resultTask;
        return result.IsSuccess ? await onSuccess() : await onFailure(result.Error);
    }

    /// <summary>
    ///     Matches the result to either the onSuccess or onFailure function, passing the result value to the onSuccess
    ///     function and returning a result of type TOut.
    /// </summary>
    /// <typeparam name="TIn">The type of the input result value.</typeparam>
    /// <typeparam name="TOut">The type of the result returned by the onSuccess or onFailure function.</typeparam>
    /// <param name="resultTask">The task that returns a result of type TIn.</param>
    /// <param name="onSuccess">The function to execute if the result is successful.</param>
    /// <param name="onFailure">The function to execute if the result is a failure.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task<Result<TOut>> Match<TIn, TOut>(this Task<Result<TIn>> resultTask,
        Func<TIn, Task<Result<TOut>>> onSuccess,
        Func<ResultError, Task<Result<TOut>>> onFailure)
    {
        var result = await resultTask;
        return result.IsSuccess ? await onSuccess(result.Value) : await onFailure(result.Error);
    }
}
