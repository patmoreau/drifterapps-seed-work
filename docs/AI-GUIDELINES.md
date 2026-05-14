# AI Guidelines — DrifterApps Seeds

Guidelines for AI assistants (and developers) working with or generating code that uses the `DrifterApps.Seeds.*` libraries.

---

## Library Purpose

These packages provide opinionated building blocks for DDD/ASP.NET Core applications:
- **Domain** — strongly-typed IDs, aggregate root contracts, unit-of-work
- **Application** — query pagination, authorization composition, JSON/EF Core converters, endpoint filters
- **Application.Mediatr** — MediatR pipeline behaviors (logging, validation, unit-of-work)
- **Infrastructure** — Hangfire-backed job scheduler, Refit response helpers
- **Testing** — Bogus builders, Testcontainers database drivers, WireMock authority, JWT helpers

The target audience is teams building multi-layered ASP.NET Core APIs with DDD and Vertical Slice Architecture.

---

## Common Patterns

### Strongly-typed identifiers

Every aggregate has a dedicated ID type — never a raw `Guid`.

```csharp
// Correct
public record OrderId : StronglyTypedId<OrderId>;
public record CustomerId : StronglyTypedId<CustomerId>;

// Incorrect — raw Guid is ambiguous at call sites
public Guid OrderId { get; init; }
```

The base record provides `New`, `Empty`, `Create(Guid)`, `Parse`, `TryParse`, comparison operators, and implicit `Guid`/`string` conversions. Derive and declare — nothing else is needed.

### Result pattern

All fallible operations return `Result<T>` from `DrifterApps.Seeds.FluentResult`. Never throw from application/domain logic for expected failure cases.

```csharp
// Correct — handler returns Result<T>
public async Task<Result<OrderDto>> Handle(GetOrderQuery query, CancellationToken ct)
{
    var order = await _repository.FindAsync(query.Id, ct);
    return order is null
        ? ResultError.NotFound("Order.NotFound", $"Order {query.Id} not found")
        : Result<OrderDto>.Success(MapToDto(order));
}

// Incorrect — throwing for expected outcomes
throw new OrderNotFoundException(query.Id);
```

The `ValidationBehavior` automatically maps FluentValidation failures to `Result<T>` before your handler runs. The `UnitOfWorkBehavior` rolls back the transaction on any exception.

### Query pagination

Build query requests by implementing `IRequestQuery` and validating with `QueryParams.Create()`. Do not pass raw `int offset, int limit` through your stack.

```csharp
// Correct
var result = QueryParams.Create(request);  // returns Result<QueryParams>
if (result.IsFailure) return result.Error.ToProblemDetails();

var items = await dbContext.Orders.Query(result.Value).ToListAsync(ct);

// Incorrect — bypassing validation
var items = await dbContext.Orders
    .Skip(request.Offset)
    .Take(request.Limit)
    .ToListAsync(ct);
```

### MediatR pipeline registration

Always call `RegisterServicesFromApplicationSeeds()` first. Add your own behaviors after it.

```csharp
services.AddMediatR(config =>
{
    config.RegisterServicesFromApplicationSeeds();          // Seeds behaviors first
    config.AddOpenBehavior(typeof(MyCustomBehavior<,>));   // Your behaviors after
    config.RegisterServicesFromAssemblyContaining<MyAssemblyMarker>();
});
```

### Unit-of-work commands

Mark commands that mutate state with `IUnitOfWorkRequest`. The `UnitOfWorkBehavior` opens the transaction and commits (or rolls back) around the handler automatically.

```csharp
public record CreateOrderCommand(CustomerId CustomerId, ...) : IUnitOfWorkRequest, IRequest<Result<OrderId>>;
```

Query handlers should not implement `IUnitOfWorkRequest` — they don't mutate state.

### Authorization composition

Use `MultiplePoliciesRequirement` to AND or OR existing policies rather than duplicating policy logic.

```csharp
// All of: user must satisfy both Admin AND Moderator
options.AddPolicy("AdminModerator", policy =>
    policy.AddRequirements(MultiplePoliciesRequirement.ForAllOf("Admin", "Moderator")));

// Any of: user must satisfy at least one
options.AddPolicy("AdminOrModerator", policy =>
    policy.AddRequirements(MultiplePoliciesRequirement.ForAnyOf("Admin", "Moderator")));

// Register the handler (once, globally)
services.AddSingleton<IAuthorizationHandler, MultiplePoliciesHandler>();
```

---

## Anti-Patterns

### Using raw Guid for entity identity

**Wrong:**
```csharp
public Guid Id { get; init; } = Guid.NewGuid();
```

**Right:**
```csharp
public OrderId Id { get; init; } = OrderId.New;
```

### Throwing from domain/application logic for expected outcomes

**Wrong:**
```csharp
throw new ValidationException("Name is required");
```

**Right:**
```csharp
return ResultError.Validation("Order.NameRequired", "Name is required");
```

### Manually specifying sort/filter strings without validation

**Wrong:**
```csharp
var sorted = query.OrderBy(filter.Sort);
```

**Right:**
```csharp
var queryParams = QueryParams.Create(offset, limit, sort, filter).Value;
var sorted = dbContext.Orders.Query(queryParams);
```

### Adding behaviors before `RegisterServicesFromApplicationSeeds`

Pipeline order matters. Logging wraps everything, then unit-of-work, then validation. Inserting a behavior before the seeds registration changes that order.

### Calling `IRepository<T>` for reads

The repository has only `SaveAsync`. Read from `DbContext`, Dapper, or a dedicated query service directly in query handlers.

### Specifying NuGet versions in .csproj

All version pins belong in `Directory.Packages.props`. Adding `Version="x.y.z"` to a `<PackageReference>` in a `.csproj` violates central package management and causes a build error.

---

## Performance Considerations

### QueryParams filter and sort

`QueryableExtensions.Query<T>` uses `System.Linq.Dynamic.Core` to translate string-based sort and filter expressions into SQL predicates. This means:

- **Always index** columns you expose as filterable or sortable. An unindexed filter on a large table will full-scan.
- **Limit filter-able columns** in your `IRequestQuery` implementations. Do not allow arbitrary column names through without a whitelist.
- **Avoid over-fetching.** Use `Limit` conservatively; avoid `DefaultLimit = int.MaxValue` on large tables.

### StronglyTypedId with EF Core

Register `StronglyTypedIdValueConverter<TId>` in `OnModelCreating` for each ID property. Without it, EF Core serializes the full record as JSON, causing inefficient storage and broken queries.

```csharp
builder.Property(x => x.Id)
    .HasConversion(new StronglyTypedIdValueConverter<OrderId>());
```

### Hangfire job serialization

`RequestScheduler.QueueHandler` uses Hangfire's JSON serialization. Large argument objects increase the size of the job payload stored in the Hangfire database. Keep job arguments small — pass IDs, not full entities.

### UnitOfWorkBehavior transaction scope

`UnitOfWorkBehavior` wraps the entire handler in a transaction. Avoid doing slow or non-transactional work (HTTP calls, file I/O) inside a `IUnitOfWorkRequest` handler. Move external calls outside the unit-of-work boundary or use a saga/outbox pattern.

---

## Integration Gotchas

### JSON serialization of StronglyTypedId

Register `StronglyTypedIdJsonConverterFactory` in `JsonSerializerOptions` or via `AddJsonOptions`. Without it, strongly-typed IDs serialize as `{"value":"..."}` instead of `"..."`.

```csharp
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new StronglyTypedIdJsonConverterFactory()));
```

Or on the MVC side:
```csharp
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new StronglyTypedIdJsonConverterFactory()));
```

### EF Core and StronglyTypedId querying

`StronglyTypedIdValueConverter<T>` allows EF Core to persist and query strongly-typed IDs. However, LINQ queries that call `.Value` on the ID inside a translated expression may not translate correctly in older EF Core versions. Prefer:

```csharp
// Correct — EF Core translates this via the converter
var order = await dbContext.Orders.FirstOrDefaultAsync(o => o.Id == orderId, ct);

// Avoid — accessing .Value may not translate
var order = await dbContext.Orders.FirstOrDefaultAsync(o => o.Id.Value == orderId.Value, ct);
```

### FluentValidation and nullable reference types

`QueryValidatorRoot<TRequest>` uses `AbstractValidator<TRequest>`. Derived validators must call `base` rule sets via the inherited rules — do not duplicate `offset >= 0` or `limit > 0` rules in subclasses.

### AuthorityDriver mock and JWT validation

`AuthorityDriver` mocks a full OpenID Connect authority including JWKS. Your test `WebApplicationFactory` must override the authority URL to point at the mock server before the application starts. Override `IConfigurationRoot` or use `ConfigureAppConfiguration`.

### Testcontainers and `InitializeAsync`

`DatabaseDriver<TDbContext>` implements `IAsyncLifetime`. In xUnit v3, use `[Collection]` fixtures or `IAsyncLifetime` on the test class directly. Do not call `InitializeAsync` manually — xUnit calls it before each test class runs.

---

## Code Style Preferences

- No comments unless the *why* is non-obvious. Well-named types document themselves.
- Nullable reference types enabled everywhere; handle nullability explicitly.
- Use `record` for IDs and value objects, `class` for services, `sealed` wherever inheritance is not intended.
- Prefer `Result<T>` over `bool` + out-parameter for fallible returns.
- `async`/`await` all the way down; no `.Result` or `.Wait()` in library code.
- One class/interface per file; file name matches type name.
- Validate at system boundaries (HTTP endpoints, job entry points). Trust internal contracts.

---

## Recommended Query Patterns

### Minimal API endpoint with pagination

```csharp
app.MapGet("/orders", async (HttpContext ctx, IMediator mediator, CancellationToken ct) =>
{
    var request = await ctx.ToQueryRequest<GetOrdersQuery>(
        (offset, limit, sort, filter) => new GetOrdersQuery(offset, limit, sort, filter));

    if (request is null) return Results.BadRequest();

    var result = await mediator.Send(request, ct);
    return result.IsSuccess
        ? Results.Ok(result.Value)
        : result.Error.ToProblemDetails();
});
```

### Handler using QueryParams

```csharp
public async Task<Result<QueryResult<OrderDto>>> Handle(GetOrdersQuery query, CancellationToken ct)
{
    var paramsResult = QueryParams.Create(query);
    if (paramsResult.IsFailure) return paramsResult.Error;

    var queryable = _dbContext.Orders.AsNoTracking();
    var total = await queryable.CountAsync(ct);
    var items = await queryable.Query(paramsResult.Value).ToListAsync(ct);

    return Result<QueryResult<OrderDto>>.Success(new QueryResult<OrderDto>(total, items.Select(MapToDto)));
}
```
