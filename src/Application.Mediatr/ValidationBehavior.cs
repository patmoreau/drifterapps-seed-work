using System.Reflection;
using DrifterApps.Seeds.Domain;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace DrifterApps.Seeds.Application.Mediatr;

/// <summary>
/// A behavior for MediatR pipeline that handles validation of requests.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseRequest
{
    private readonly IReadOnlyCollection<IValidator<TRequest>> _validators =
        validators.ToList() ?? throw new ArgumentNullException(nameof(validators));

    /// <summary>
    /// Handles the request and performs validation.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The response from the next delegate or validation errors.</returns>
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(next);

        return HandleInternal(request, next, cancellationToken);
    }

    /// <summary>
    /// Internal method to handle the request and perform validation.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The response from the next delegate or validation errors.</returns>
    private async Task<TResponse> HandleInternal(TRequest request,
        RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var validationFailures = await Validate(request, cancellationToken).ConfigureAwait(false);
        if (validationFailures.Count != 0)
        {
            return ReturnValidationErrors(validationFailures);
        }

        return await next().ConfigureAwait(false);
    }

    /// <summary>
    /// Validates the request using the provided validators.
    /// </summary>
    /// <param name="request">The request to validate.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of validation failures.</returns>
    private async Task<IReadOnlyList<ValidationFailure>> Validate(TRequest request, CancellationToken cancellationToken)
    {
        if (_validators.Count == 0)
        {
            return [];
        }

        var validationContext = new ValidationContext<TRequest>(request);
        var failures = await Task
            .WhenAll(_validators.Select(v => v.ValidateAsync(validationContext, cancellationToken)))
            .ConfigureAwait(false);

        return failures.SelectMany(result => result.Errors).Where(failure => failure != null).ToList();
    }

    /// <summary>
    /// Returns the validation errors as a response.
    /// </summary>
    /// <param name="validationFailures">The list of validation failures.</param>
    /// <returns>The response containing validation errors.</returns>
    private static TResponse ReturnValidationErrors(IReadOnlyList<ValidationFailure> validationFailures) =>
        typeof(TResponse) switch
        {
            {IsGenericType: true} t when t.GetGenericTypeDefinition() == typeof(Result<>) =>
                CreateGenericResultFailureInstance(validationFailures),
            { } t when t == typeof(Result) => CreateResultFailureInstance(validationFailures),
            _ => throw new ValidationException(validationFailures)
        };

    /// <summary>
    /// Creates an instance of a failure result for non-generic Result type.
    /// </summary>
    /// <param name="validationFailures">The list of validation failures.</param>
    /// <returns>The failure result instance.</returns>
    private static TResponse CreateResultFailureInstance(IReadOnlyList<ValidationFailure> validationFailures)
    {
        var failureInstance = typeof(Result)
            .GetMethod("Failure", BindingFlags.Static | BindingFlags.Public)
            ?.Invoke(null, [ResultValidationErrors.ValidationErrors(typeof(TRequest), validationFailures)]);

        return (TResponse) failureInstance!;
    }

    /// <summary>
    /// Creates an instance of a failure result for generic Result type.
    /// </summary>
    /// <param name="validationFailures">The list of validation failures.</param>
    /// <returns>The failure result instance.</returns>
    private static TResponse CreateGenericResultFailureInstance(IReadOnlyList<ValidationFailure> validationFailures)
    {
        var genericType = typeof(TResponse).GetGenericArguments()[0];
        var failureInstance = typeof(Result<>)
            .MakeGenericType(genericType)
            .GetMethod("Failure", BindingFlags.Static | BindingFlags.Public)
            ?.Invoke(null, [ResultValidationErrors.ValidationErrors(typeof(TRequest), validationFailures)]);

        return (TResponse) failureInstance!;
    }
}