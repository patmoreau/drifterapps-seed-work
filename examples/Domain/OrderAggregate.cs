using DrifterApps.Seeds.Domain;

namespace SampleApp.Domain;

// One-line strongly-typed IDs — the compiler prevents passing a CustomerId where an OrderId is expected.
public record OrderId : StronglyTypedId<OrderId>;
public record CustomerId : StronglyTypedId<CustomerId>;

public class Order : IAggregateRoot<OrderId>
{
    public OrderId Id { get; private set; } = OrderId.New;
    public CustomerId CustomerId { get; private set; } = null!;
    public decimal Total { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private Order() { }  // Required by EF Core

    public static Order Create(CustomerId customerId, decimal total)
    {
        ArgumentNullException.ThrowIfNull(customerId);
        if (total <= 0) throw new ArgumentOutOfRangeException(nameof(total), "Total must be positive");

        return new Order
        {
            CustomerId = customerId,
            Total = total,
            Status = OrderStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public void Ship()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException($"Cannot ship an order in status {Status}");
        Status = OrderStatus.Shipped;
    }
}

public enum OrderStatus { Pending, Shipped, Cancelled }
