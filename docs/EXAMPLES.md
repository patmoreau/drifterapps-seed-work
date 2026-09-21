# Usage Examples — DrifterApps Seeds

End-to-end examples showing how the `DrifterApps.Seeds.*` packages work together in a typical DDD/Vertical Slice ASP.NET Core application.

The examples use an **Order management** domain to stay consistent across sections.

---

## 1. Domain Layer

### Defining a strongly-typed identifier

```csharp
// One line — no body needed.
public record OrderId : StronglyTypedId<OrderId>;
public record CustomerId : StronglyTypedId<CustomerId>;
```

Usage:

```csharp
var id = OrderId.New;                            // New GUID
var empty = OrderId.Empty;                       // Guid.Empty
var known = OrderId.Create(Guid.Parse("..."));   // Specific GUID
var parsed = OrderId.Parse(guidString, null);    // From string; Empty on failure
bool ok = OrderId.TryParse(guidString, null, out var result);

// Implicit conversions keep consumer code clean
Guid raw = id;                  // implicit → Guid
OrderId fromGuid = someGuid;    // implicit ← Guid
```

### Defining an aggregate root

```csharp
public class Order : IAggregateRoot<OrderId>
{
    public OrderId Id { get; private set; } = OrderId.New;
    public CustomerId CustomerId { get; private set; }
    public decimal Total { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private Order() { }  // EF Core

    public static Order Create(CustomerId customerId, decimal total)
    {
        ArgumentNullException.ThrowIfNull(customerId);
        return new Order
        {
            CustomerId = customerId,
            Total = total,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}
```

### Repository contract

```csharp
public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> FindAsync(OrderId id, CancellationToken cancellationToken = default);
}
```

Only `SaveAsync` (from `IRepository<T>`) belongs to the write side. `FindAsync` is added by the concrete interface for query use-cases.

---

## 2. Application Layer

### EF Core configuration for strongly-typed IDs

```csharp
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id)
            .HasConversion(new StronglyTypedIdValueConverter<OrderId>());
        builder.Property(o => o.CustomerId)
            .HasConversion(new StronglyTypedIdValueConverter<CustomerId>());
    }
}
```

### JSON serialization for strongly-typed IDs

Register once in `Program.cs`:

```csharp
builder.Services.ConfigureHttpJsonOptions(opts =>
    opts.SerializerOptions.Converters.Add(new StronglyTypedIdJsonConverterFactory()));
```

Now `OrderId` serializes as `"3fa85f64-5717-4562-b3fc-2c963f66afa6"` instead of `{"value":"..."}`.

### Query request and validator

```csharp
public record GetOrdersQuery(
    int Offset, int Limit, string[] Sort, string[] Filter)
    : IRequestQuery;

public class GetOrdersQueryValidator : QueryValidatorRoot<GetOrdersQuery>
{
    public GetOrdersQueryValidator()
    {
        // offset >= 0 and limit > 0 are inherited from QueryValidatorRoot
        RuleFor(q => q.Sort)
            .Must(s => s.All(v => v is "-total" or "total" or "-createdAt" or "createdAt"))
            .WithMessage("Sort accepts: total, -total, createdAt, -createdAt");
    }
}
```

### Query handler

```csharp
public class GetOrdersHandler(AppDbContext dbContext)
{
    public async Task<Result<QueryResult<OrderDto>>> HandleAsync(
        GetOrdersQuery query, CancellationToken cancellationToken)
    {
        var paramsResult = QueryParams.Create(query);
        if (paramsResult.IsFailure)
            return paramsResult.Error;

        var queryable = dbContext.Orders.AsNoTracking();
        var total = await queryable.CountAsync(cancellationToken);
        var items = await queryable
            .Query(paramsResult.Value)
            .Select(o => new OrderDto(o.Id, o.CustomerId, o.Total, o.CreatedAt))
            .ToListAsync(cancellationToken);

        return new QueryResult<OrderDto>(total, items);
    }
}
```

### Command with unit-of-work

```csharp
public record CreateOrderCommand(CustomerId CustomerId, decimal Total);

public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(c => c.CustomerId).NotEmpty();
        RuleFor(c => c.Total).GreaterThan(0);
    }
}

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
```

`UnitOfWorkFilter` on the endpoint begins the transaction before the handler runs
and commits after it returns; any exception rolls it back. `ValidationFilter<TRequest>`
runs `CreateOrderValidator` first, so the transaction never opens for an invalid request.

### Minimal API endpoint

```csharp
app.MapGet("/orders", async (HttpContext ctx, GetOrdersHandler handler, CancellationToken ct) =>
{
    var request = await ctx.ToQueryRequest<GetOrdersQuery>(
        (offset, limit, sort, filter) => new GetOrdersQuery(offset, limit, sort, filter));

    if (request is null) return Results.BadRequest();

    var result = await handler.HandleAsync(request, ct);
    return result.IsSuccess
        ? Results.Ok(result.Value)
        : result.Error.ToProblemDetails(StatusCodes.Status400BadRequest);
})
.WithName("GetOrders")
.Produces<QueryResult<OrderDto>>()
.ProducesValidationProblem();

app.MapPost("/orders", async (CreateOrderCommand command, CreateOrderHandler handler, CancellationToken ct) =>
{
    var result = await handler.HandleAsync(command, ct);
    return result.IsSuccess
        ? Results.Created($"/orders/{result.Value}", result.Value)
        : result.Error.ToProblemDetails(StatusCodes.Status400BadRequest);
})
.AddEndpointFilter<ValidationFilter<CreateOrderCommand>>()
.AddEndpointFilter<UnitOfWorkFilter>()
.WithName("CreateOrder");
```

### Authorization with multiple policies

```csharp
// In Program.cs
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanManageOrders", policy =>
        policy.AddRequirements(
            MultiplePoliciesRequirement.ForAnyOf("Admin", "OrderManager")));
});
builder.Services.AddSingleton<IAuthorizationHandler, MultiplePoliciesHandler>();

// On the endpoint
app.MapPost("/orders", ...)
    .RequireAuthorization("CanManageOrders");
```

---

## 3. Endpoint Filters

Cross-cutting concerns attach per endpoint, outermost filter first:

```csharp
app.MapPost("/orders", CreateOrder)
    .AddEndpointFilter<ValidationFilter<CreateOrderCommand>>()   // runs first
    .AddEndpointFilter<UnitOfWorkFilter>()                       // runs second
    .RequireAuthorization("CanManageOrders");
```

What each one does:

| Filter | Effect |
|---|---|
| `ValidationFilter<TRequest>` | Resolves `IValidator<TRequest>`; on failure short-circuits with `ValidationProblem` (or `Problem` 500 when the error is not an aggregate) and never calls the endpoint |
| `UnitOfWorkFilter` | Resolves `IUnitOfWork`, `BeginWorkAsync` → endpoint → `CommitWorkAsync`, `RollbackWorkAsync` and rethrow on any exception |

Order matters: put `ValidationFilter<TRequest>` before `UnitOfWorkFilter` so an invalid
request never opens a transaction. `ValidationFilter<TRequest>` is a no-op when the
request type is absent from the arguments or no validator is registered for it.

Both are plain `IEndpointFilter` implementations — add your own alongside them for
logging, metrics or anything else the endpoint needs.

## 4. Infrastructure — Background Jobs

```csharp
// Registration
builder.Services.AddHangfireRequestScheduler(
    () => new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

// Usage in a handler
public class ShipOrderHandler(IRequestScheduler scheduler)
{
    public Result<Nothing> Handle(ShipOrderCommand command)
    {
        // Enqueues a Hangfire background job
        scheduler.QueueHandler<IEmailService>(
            svc => svc.SendShipmentConfirmationAsync(command.OrderId),
            "Send shipment confirmation email");

        return Nothing.Value;
    }
}
```

---

## 5. Testing

### Fake data builder

```csharp
public class FakeOrderBuilder : FakerBuilder<Order>
{
    protected override Faker<Order> Faker => CreateUninitializedFaker<Order>()
        .RuleFor(o => o.Id, _ => OrderId.New)
        .RuleFor(o => o.CustomerId, _ => CustomerId.New)
        .RuleFor(o => o.Total, f => f.Finance.Amount(1, 10_000))
        .RuleFor(o => o.CreatedAt, f => f.Date.PastOffset());

    public FakeOrderBuilder WithCustomer(CustomerId customerId)
    {
        Faker.RuleFor(o => o.CustomerId, _ => customerId);
        return this;
    }
}

// In tests
var order = new FakeOrderBuilder().Build();
var orders = new FakeOrderBuilder().BuildCollection(count: 5);
var customerOrders = new FakeOrderBuilder()
    .WithCustomer(knownCustomerId)
    .BuildCollection(count: 3);
```

### Integration test with a real database

```csharp
public class PostgresOrderDatabaseDriver(PostgreSqlContainer container)
    : DatabaseDriver<AppDbContext>
{
    protected override IDatabaseServer DatabaseServer { get; init; }
        = new PostgreDatabaseServer(container);
}

[Collection("Database")]
public class OrderRepositoryTests(PostgresOrderDatabaseDriver driver) : IAsyncLifetime
{
    public ValueTask InitializeAsync() => driver.InitializeAsync();
    public ValueTask DisposeAsync() => driver.DisposeAsync();

    [ComponentTest]
    public async Task SaveAsync_PersistsOrder()
    {
        var order = new FakeOrderBuilder().Build();
        await driver.SaveAsync(order);

        var found = await driver.FindAsync<Order>(order.Id.Value);
        found.Should().NotBeNull();
        found!.Total.Should().Be(order.Total);
    }
}
```

### Mock authority and JWT tokens (integration tests with authentication)

```csharp
public class AuthenticatedApiTests : IAsyncLifetime
{
    private readonly AuthorityDriver _authority = new();
    private readonly WebApplicationFactory<Program> _factory;

    public AuthenticatedApiTests()
    {
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            builder.ConfigureAppConfiguration((_, cfg) =>
                cfg.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Authentication:Authority"] = _authority.Authority.ToString()
                })));
    }

    public async ValueTask InitializeAsync() => await _authority.InitializeAsync();
    public async ValueTask DisposeAsync()
    {
        await _authority.DisposeAsync();
        await _factory.DisposeAsync();
    }

    [EndToEndTest]
    public async Task GetOrders_WithValidToken_Returns200()
    {
        var token = new JwtTokenBuilder()
            .ForAudience("my-api")
            .IssuedBy(_authority.Authority.ToString())
            .ExpireIn(TimeSpan.FromMinutes(5))
            .WithRole("OrderManager")
            .Build();

        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/orders");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
```

### FluentAssertions for Refit API responses

```csharp
// Assuming IOrderApi is a Refit interface
var response = await orderApi.GetAsync(OrderId.New);

response.Should().HaveContent();                           // non-null body
response.Should().HaveContent(expectedDto);                // exact match
response.Should().HaveEquivalentContent(expectedDto,       // structural match
    cfg => cfg.Excluding(o => o.CreatedAt));
```

---

## 6. Filter and Sort Examples

Filters and sort strings can be passed directly from HTTP query strings.

### HTTP request examples

```
GET /orders?offset=0&limit=20&sort=-createdAt&filter=total:gt:100
GET /orders?offset=20&limit=20&sort=total&sort=-createdAt&filter=customerId:eq:3fa85f64-5717-4562-b3fc-2c963f66afa6
```

### Filter operators

| Operator | Meaning | Example |
|---|---|---|
| `eq` | Equal | `status:eq:Shipped` |
| `ne` | Not equal | `status:ne:Cancelled` |
| `lt` | Less than | `total:lt:500` |
| `le` | Less than or equal | `total:le:500` |
| `gt` | Greater than | `total:gt:100` |
| `ge` | Greater than or equal | `total:ge:100` |

### Sort syntax

| Value | Order |
|---|---|
| `total` | Ascending |
| `-total` | Descending |
| `createdAt` | Ascending |
| `-createdAt` | Descending |

Multiple sort expressions are supported — pass `sort` multiple times in the query string.
