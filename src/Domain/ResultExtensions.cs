using System.Diagnostics.CodeAnalysis;

namespace DrifterApps.Seeds.Domain;

[SuppressMessage("Minor Code Smell", "S4136:Method overloads should be grouped together")]
[SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
[SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task")]
public static partial class ResultExtensions
{
    public static Result OnSuccess(this Result result, Func<Result> next) =>
        result.IsSuccess ? next() : result;

    public static Result<TOut> OnSuccess<TOut>(this Result result, Func<Result<TOut>> next) =>
        result.IsSuccess ? next() : Result<TOut>.Failure(result.Error);

    public static Result OnSuccess<TIn>(this Result<TIn> result, Func<TIn, Result> next) =>
        result.IsSuccess ? next(result.Value) : Result.Failure(result.Error);

    public static Result<TOut> OnSuccess<TIn, TOut>(this Result<TIn> result, Func<TIn, Result<TOut>> next) =>
        result.IsSuccess ? next(result.Value) : Result<TOut>.Failure(result.Error);

    public static Result OnFailure(this Result result, Func<ResultError, Result> next) =>
        result.IsFailure ? next(result.Error) : result;

    public static Result OnFailure<TIn>(this Result<TIn> result, Func<ResultError, Result> next) =>
        result.IsFailure ? next(result.Error) : result;

    public static Result Match(this Result result, Func<Result> onSuccess, Func<ResultError, Result> onFailure) =>
        result.IsSuccess ? onSuccess() : onFailure(result.Error);

    public static Result Match<TIn>(this Result<TIn> result, Func<TIn, Result> onSuccess,
        Func<ResultError, Result> onFailure) =>
        result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error);

    public static Result<TOut> Match<TOut>(this Result result, Func<Result<TOut>> onSuccess,
        Func<ResultError, Result<TOut>> onFailure) =>
        result.IsSuccess ? onSuccess() : onFailure(result.Error);

    public static Result<TOut> Match<TIn, TOut>(this Result<TIn> result, Func<TIn, Result<TOut>> onSuccess,
        Func<ResultError, Result<TOut>> onFailure) =>
        result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error);
}
