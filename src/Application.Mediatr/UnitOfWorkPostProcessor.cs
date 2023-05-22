using DrifterApps.Seeds.Domain;
using MediatR.Pipeline;

namespace DrifterApps.Seeds.Application.Mediatr;

/// <summary>
/// <see cref="IUnitOfWork"/> post processor to commit work
/// </summary>
/// <typeparam name="TRequest">Request type implementing <see cref="IUnitOfWorkRequest"/></typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
public sealed class UnitOfWorkPostProcessor<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
    where TRequest : IUnitOfWorkRequest
{
    private readonly IUnitOfWork _unitOfWork;

    public UnitOfWorkPostProcessor(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    /// <inheritdoc />
    public async Task Process(TRequest request, TResponse response, CancellationToken cancellationToken) =>
        await _unitOfWork.CommitWorkAsync(cancellationToken).ConfigureAwait(false);
}
