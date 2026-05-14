using DrifterApps.Seeds.Application.Mediatr;
using DrifterApps.Seeds.FluentResult;
using FluentValidation;
using MediatR;
using SampleApp.Domain;

namespace SampleApp.Application.Orders;

// IUnitOfWorkRequest tells UnitOfWorkBehavior to open and commit a transaction around this handler.
public record CreateOrderCommand(CustomerId CustomerId, decimal Total)
    : IUnitOfWorkRequest, IRequest<Result<OrderId>>;

public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(c => c.CustomerId).NotEmpty();
        RuleFor(c => c.Total).GreaterThan(0).WithMessage("Total must be positive");
    }
}

// ValidationBehavior runs before this handler; the command is already valid when Handle() is called.
public class CreateOrderHandler(IOrderRepository repository)
    : IRequestHandler<CreateOrderCommand, Result<OrderId>>
{
    public async Task<Result<OrderId>> Handle(
        CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var order = Order.Create(command.CustomerId, command.Total);
        await repository.SaveAsync(order, cancellationToken);
        return Result<OrderId>.Success(order.Id);
    }
}

public interface IOrderRepository : DrifterApps.Seeds.Domain.IRepository<Order>
{
    Task<Order?> FindAsync(OrderId id, CancellationToken cancellationToken = default);
}
