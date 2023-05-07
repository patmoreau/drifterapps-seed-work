using MediatR;

namespace DrifterApps.Seeds.Application;

#pragma warning disable CA1040
/// <inheritdoc />
public interface ICommandRequest<out TResponse> : IRequest<TResponse>
{
}

public interface ICommandRequest : IRequest
{
}
#pragma warning restore CA1040
