using DrifterApps.Seeds.Domain;
using MediatR.Pipeline;

namespace DrifterApps.Seeds.Application.Mediatr;

/// <summary>
/// <see cref="IUnitOfWork"/> pre processor to begin work
/// </summary>
/// <typeparam name="TRequest">Request type implementing <see cref="IUnitOfWorkRequest"/></typeparam>
public sealed class UnitOfWorkPreProcessor<TRequest> : IRequestPreProcessor<TRequest> where TRequest : IUnitOfWorkRequest
{
    private readonly IUnitOfWork _unitOfWork;

    public UnitOfWorkPreProcessor(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    /// <inheritdoc />
    public async Task Process(TRequest request, CancellationToken cancellationToken) =>
        await _unitOfWork.BeginWorkAsync(cancellationToken).ConfigureAwait(false);
}
