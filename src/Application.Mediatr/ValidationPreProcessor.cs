using FluentValidation;
using MediatR.Pipeline;

namespace DrifterApps.Seeds.Application.Mediatr;

public sealed class ValidationPreProcessor<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly IReadOnlyCollection<IValidator<TRequest>> _validators;

    public ValidationPreProcessor(IReadOnlyCollection<IValidator<TRequest>> validators) =>
        _validators = validators ?? throw new ArgumentNullException(nameof(validators));

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        if (_validators.Count == 0)
        {
            return;
        }

        var validationContext = new ValidationContext<TRequest>(request);
        var failures = await Task
            .WhenAll(_validators.Select(v => v.ValidateAsync(validationContext, cancellationToken)))
            .ConfigureAwait(false);

        var validationFailures =
            failures.SelectMany(result => result.Errors).Where(failure => failure != null).ToList();

        if (validationFailures.Count != 0)
        {
            throw new ValidationException(validationFailures);
        }
    }
}
