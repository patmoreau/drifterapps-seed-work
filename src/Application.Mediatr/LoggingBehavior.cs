using MediatR;
using Microsoft.Extensions.Logging;

namespace DrifterApps.Seeds.Application.Mediatr;

/// <summary>
///     Logging behavior to log the start and end of a request
/// </summary>
/// <typeparam name="TRequest">Request type</typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
public sealed class LoggingBehavior<TRequest, TResponse>(ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private static readonly string RequestName = typeof(TRequest).FullName ?? nameof(TRequest);

    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(next);

        return HandleInternal(request, next);
    }

    private async Task<TResponse> HandleInternal(TRequest request, RequestHandlerDelegate<TResponse> next)
    {
        using var scope = logger.BeginScope(request);

        logger.LogInformation("API Request started: {Name} {@Request}", RequestName, request);
        var response = await next().ConfigureAwait(false);
        logger.LogInformation("API Request ended: {Name} with result {@Response}", RequestName, response);

        return response;
    }
}
