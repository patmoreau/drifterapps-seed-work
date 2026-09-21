using DrifterApps.Seeds.FluentResult;
using FluentValidation;
using SampleApp.Domain;

namespace SampleApp.Application.Orders;

// UnitOfWorkFilter on the endpoint opens and commits a transaction around this handler.
public record CreateOrderCommand(CustomerId CustomerId, decimal Total);

public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(c => c.CustomerId).NotEmpty();
        RuleFor(c => c.Total).GreaterThan(0).WithMessage("Total must be positive");
    }
}

// ValidationFilter runs before this handler; the command is already valid when HandleAsync() is called.
public class CreateOrderHandler(IOrderRepository repository)
{
    public async Task<Result<OrderId>> HandleAsync(
        CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var order = Order.Create(command.CustomerId, command.Total);
        await repository.SaveAsync(order, cancellationToken);
        return order.Id;
    }
}

public interface IOrderRepository : DrifterApps.Seeds.Domain.IRepository<Order>
{
    Task<Order?> FindAsync(OrderId id, CancellationToken cancellationToken = default);
}
