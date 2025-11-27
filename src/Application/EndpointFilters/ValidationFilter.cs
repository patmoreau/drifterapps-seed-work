using DrifterApps.Seeds.Application.Extensions;
using DrifterApps.Seeds.FluentResult;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DrifterApps.Seeds.Application.EndpointFilters;

public class ValidationFilter<TRequest> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);

        var request = context.Arguments.OfType<TRequest>().FirstOrDefault();
        if (request is null)
        {
            return await next(context).ConfigureAwait(false);
        }

        var validator = context.HttpContext.RequestServices.GetService<IValidator<TRequest>>();
        if (validator is null)
        {
            return await next(context).ConfigureAwait(false);
        }

        var cancellationToken = context.HttpContext.RequestAborted;
        var validationResult = await validator.IsValidAsync(request, cancellationToken).ConfigureAwait(false);

        if (validationResult.IsFailure)
        {
            var errors = validationResult.Error as ResultErrorAggregate;
            return errors?.ToValidationProblemDetails() ??
                   validationResult.Error.ToProblemDetails(StatusCodes.Status500InternalServerError);
        }

        return await next(context).ConfigureAwait(false);
    }
}
