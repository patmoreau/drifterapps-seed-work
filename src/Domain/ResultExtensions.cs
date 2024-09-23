using System.Diagnostics.CodeAnalysis;

namespace DrifterApps.Seeds.Domain;

/// <summary>
///     Provides extension methods for handling results.
/// </summary>
[SuppressMessage("Minor Code Smell", "S4136:Method overloads should be grouped together")]
[SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
[SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task")]
public static partial class ResultExtensions
{
    /// <summary>
    ///     Executes the next function if the result is successful.
    /// </summary>
    /// <param name="result">The initial result.</param>
    /// <param name="next">The function to execute if the result is successful.</param>
    /// <returns>The result of the next function or the initial result.</returns>
    public static Result OnSuccess(this Result result, Func<Result> next) =>
        result.IsSuccess ? next() : result;

    /// <summary>
    ///     Executes the next function if the result is successful and returns a result of type <typeparamref name="TOut" />.
    /// </summary>
    /// <typeparam name="TOut">The type of the result.</typeparam>
    /// <param name="result">The initial result.</param>
    /// <param name="next">The function to execute if the result is successful.</param>
    /// <returns>The result of the next function or a failure result.</returns>
    public static Result<TOut> OnSuccess<TOut>(this Result result, Func<Result<TOut>> next) =>
        result.IsSuccess ? next() : Result<TOut>.Failure(result.Error);

    /// <summary>
    ///     Executes the next function if the result is successful.
    /// </summary>
    /// <typeparam name="TIn">The type of the input value.</typeparam>
    /// <param name="result">The initial result.</param>
    /// <param name="next">The function to execute if the result is successful.</param>
    /// <returns>The result of the next function or a failure result.</returns>
    public static Result OnSuccess<TIn>(this Result<TIn> result, Func<TIn, Result> next) =>
        result.IsSuccess ? next(result.Value) : Result.Failure(result.Error);

    /// <summary>
    ///     Executes the next function if the result is successful and returns a result of type <typeparamref name="TOut" />.
    /// </summary>
    /// <typeparam name="TIn">The type of the input value.</typeparam>
    /// <typeparam name="TOut">The type of the result.</typeparam>
    /// <param name="result">The initial result.</param>
    /// <param name="next">The function to execute if the result is successful.</param>
    /// <returns>The result of the next function or a failure result.</returns>
    public static Result<TOut> OnSuccess<TIn, TOut>(this Result<TIn> result, Func<TIn, Result<TOut>> next) =>
        result.IsSuccess ? next(result.Value) : Result<TOut>.Failure(result.Error);

    /// <summary>
    ///     Executes the next function if the result is a failure.
    /// </summary>
    /// <param name="result">The initial result.</param>
    /// <param name="next">The function to execute if the result is a failure.</param>
    /// <returns>The result of the next function or the initial result.</returns>
    public static Result OnFailure(this Result result, Func<ResultError, Result> next) =>
        result.IsFailure ? next(result.Error) : result;

    /// <summary>
    ///     Executes the next function if the result is a failure.
    /// </summary>
    /// <typeparam name="TIn">The type of the input value.</typeparam>
    /// <param name="result">The initial result.</param>
    /// <param name="next">The function to execute if the result is a failure.</param>
    /// <returns>The result of the next function or the initial result.</returns>
    public static Result OnFailure<TIn>(this Result<TIn> result, Func<ResultError, Result> next) =>
        result.IsFailure ? next(result.Error) : result;

    /// <summary>
    ///     Matches the result to the appropriate function based on success or failure.
    /// </summary>
    /// <param name="result">The initial result.</param>
    /// <param name="onSuccess">The function to execute if the result is successful.</param>
    /// <param name="onFailure">The function to execute if the result is a failure.</param>
    /// <returns>The result of the appropriate function.</returns>
    public static Result Match(this Result result, Func<Result> onSuccess, Func<ResultError, Result> onFailure) =>
        result.IsSuccess ? onSuccess() : onFailure(result.Error);

    /// <summary>
    ///     Matches the result to the appropriate function based on success or failure.
    /// </summary>
    /// <typeparam name="TIn">The type of the input value.</typeparam>
    /// <param name="result">The initial result.</param>
    /// <param name="onSuccess">The function to execute if the result is successful.</param>
    /// <param name="onFailure">The function to execute if the result is a failure.</param>
    /// <returns>The result of the appropriate function.</returns>
    public static Result Match<TIn>(this Result<TIn> result, Func<TIn, Result> onSuccess,
        Func<ResultError, Result> onFailure) =>
        result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error);

    /// <summary>
    ///     Matches the result to the appropriate function based on success or failure and returns a result of type
    ///     <typeparamref name="TOut" />.
    /// </summary>
    /// <typeparam name="TOut">The type of the result.</typeparam>
    /// <param name="result">The initial result.</param>
    /// <param name="onSuccess">The function to execute if the result is successful.</param>
    /// <param name="onFailure">The function to execute if the result is a failure.</param>
    /// <returns>The result of the appropriate function.</returns>
    public static Result<TOut> Match<TOut>(this Result result, Func<Result<TOut>> onSuccess,
        Func<ResultError, Result<TOut>> onFailure) =>
        result.IsSuccess ? onSuccess() : onFailure(result.Error);

    /// <summary>
    ///     Matches the result to the appropriate function based on success or failure and returns a result of type
    ///     <typeparamref name="TOut" />.
    /// </summary>
    /// <typeparam name="TIn">The type of the input value.</typeparam>
    /// <typeparam name="TOut">The type of the result.</typeparam>
    /// <param name="result">The initial result.</param>
    /// <param name="onSuccess">The function to execute if the result is successful.</param>
    /// <param name="onFailure">The function to execute if the result is a failure.</param>
    /// <returns>The result of the appropriate function.</returns>
    public static Result<TOut> Match<TIn, TOut>(this Result<TIn> result, Func<TIn, Result<TOut>> onSuccess,
        Func<ResultError, Result<TOut>> onFailure) =>
        result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error);
}
