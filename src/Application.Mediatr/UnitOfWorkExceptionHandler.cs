using DrifterApps.Seeds.Domain;
using MediatR.Pipeline;

namespace DrifterApps.Seeds.Application.Mediatr;

/// <summary>
///     <see cref="IUnitOfWork" /> exception handler to rollback work
/// </summary>
/// <typeparam name="TRequest">Request type implementing <see cref="IUnitOfWorkRequest" /></typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
/// <typeparam name="TException">Exception type</typeparam>
public sealed class UnitOfWorkExceptionHandler<TRequest, TResponse, TException>
    : IRequestExceptionHandler<TRequest, TResponse, TException>
    where TRequest : IUnitOfWorkRequest
    where TException : Exception
{
    private readonly IUnitOfWork _unitOfWork;

    public UnitOfWorkExceptionHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    /// <inheritdoc />
    public async Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state,
        CancellationToken cancellationToken) =>
        await _unitOfWork.RollbackWorkAsync(cancellationToken).ConfigureAwait(false);
}
