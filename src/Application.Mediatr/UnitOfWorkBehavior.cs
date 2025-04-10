using DrifterApps.Seeds.Domain;
using MediatR;

namespace DrifterApps.Seeds.Application.Mediatr;

/// <summary>
///     Unit of work behavior to handle when a request starts, gets committed and needs to be rolled back
/// </summary>
/// <typeparam name="TRequest">
///     Request of type <see cref="IUnitOfWorkRequest" /> and <see cref="IBaseRequest" />
/// </typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
public sealed class UnitOfWorkBehavior<TRequest, TResponse>(IUnitOfWork unitOfWork)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IUnitOfWorkRequest, IBaseRequest
{
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(next);

        return HandleInternal(next, cancellationToken);
    }

    private async Task<TResponse> HandleInternal(RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        await unitOfWork.BeginWorkAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            var response = await next(cancellationToken).ConfigureAwait(false);

            await unitOfWork.CommitWorkAsync(cancellationToken).ConfigureAwait(false);

            return response;
        }
        catch (Exception)
        {
            await unitOfWork.RollbackWorkAsync(cancellationToken).ConfigureAwait(false);
            throw;
        }
    }
}
