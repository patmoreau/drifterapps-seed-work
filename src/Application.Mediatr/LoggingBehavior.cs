using MediatR;
using Microsoft.Extensions.Logging;

namespace DrifterApps.Seeds.Application.Mediatr;

/// <summary>
///     Logging behavior to log the start and end of a request
/// </summary>
/// <typeparam name="TRequest">Request type</typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private static readonly string RequestName = typeof(TRequest).FullName ?? nameof(TRequest);

    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;

    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(next);

        return HandleInternal(request, next);
    }

    private async Task<TResponse> HandleInternal(TRequest request, RequestHandlerDelegate<TResponse> next)
    {
        using var scope = _logger.BeginScope(request);

        _logger.LogInformation("API Request started: {Name} {@Request}", RequestName, request);
        var response = await next().ConfigureAwait(false);
        _logger.LogInformation("API Request ended: {Name} with result {@Response}", RequestName, response);

        return response;
    }
}
