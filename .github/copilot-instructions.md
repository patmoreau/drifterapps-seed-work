# GitHub Copilot Instructions — DrifterApps.Seeds

Opinionated building blocks for DDD / Vertical Slice Architecture ASP.NET Core applications.

**Packages:** `DrifterApps.Seeds.Domain`, `.Application`, `.Infrastructure`, `.Testing`
**Companions:** `DrifterApps.Seeds.FluentResult`, `DrifterApps.Seeds.FluentScenario`
**Targets:** .NET 10

---

## Package Map

| Package | Use it for |
|---|---|
| `Domain` | `IAggregateRoot`, `IAggregateRoot<TId>`, `IRepository<TAggregate>`, `IUnitOfWork`, `StronglyTypedId<T>`, `IPrimitiveType<T>` |
| `Application` | `QueryParams`, `QueryResult<T>`, `IRequestQuery`, `MultiplePoliciesRequirement`, `ValidationFilter<TRequest>`, `UnitOfWorkFilter`, EF Core + JSON converters, `IHttpUserContext`, `IRequestScheduler` |
| `Infrastructure` | Hangfire-backed `IRequestScheduler`, Refit helpers, `IJsonSerializerOptionsFactory` |
| `Testing` | `FakerBuilder<T>`, `DatabaseDriver<TDbContext>`, `WireMockDriver`, `AuthorityDriver`, `FeatureManagerDriver`, `JwtTokenBuilder`, trait attributes, Refit assertions |

Dependency direction: `Domain` ← `Application` ← `Infrastructure`; `Testing` → `Domain` + `Infrastructure`.

---

## Strongly-typed identifiers

Every aggregate gets a dedicated ID type — never a raw `Guid`.

```csharp
public record OrderId : StronglyTypedId<OrderId>;
public record CustomerId : StronglyTypedId<CustomerId>;

public class Order : IAggregateRoot<OrderId>
{
    public OrderId Id { get; private set; } = OrderId.New;
    public CustomerId CustomerId { get; private set; } = null!;
}
```

The base record supplies `New`, `Empty`, `Create(Guid)`, `Parse`, `TryParse`, comparison
operators and implicit `Guid`/`string` conversions. Derive and declare — nothing else.

Wire the converters or IDs serialize as `{"value":"…"}` and break EF Core queries:

```csharp
// JSON (minimal API)
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new StronglyTypedIdJsonConverterFactory()));

// EF Core, per ID property in OnModelCreating
builder.Property(x => x.Id).HasConversion(new StronglyTypedIdValueConverter<OrderId>());
```

Compare IDs directly (`o.Id == orderId`); do not reach through `.Value` inside a
translated LINQ expression.

---

## Result pattern

Fallible operations return `Result<T>` from `DrifterApps.Seeds.FluentResult`. Failures
are values, not exceptions; `ResultError` always carries a `Code` and a `Description`,
and both directions convert implicitly.

```csharp
internal static class OrderErrors
{
    internal static ResultError NotFound(OrderId id) =>
        new("Order.NotFound", $"Order '{id}' was not found.");
}

public async Task<Result<OrderDto>> Handle(GetOrderQuery query, CancellationToken ct)
{
    var order = await _repository.FindAsync(query.Id, ct);
    return order is null
        ? OrderErrors.NotFound(query.Id)   // implicit ResultError -> Result<OrderDto>
        : MapToDto(order);                 // implicit T -> Result<OrderDto>
}
```

At the HTTP boundary:

```csharp
return result.IsSuccess
    ? Results.Ok(result.Value)
    : result.Error.ToProblemDetails(StatusCodes.Status404NotFound);

// Validation failures carry per-field errors
return aggregateError.ToValidationProblemDetails();
```

Programmer errors stay exceptions (`ArgumentNullException.ThrowIfNull`). Expected domain
outcomes never throw.

---

## Query pagination

Implement `IRequestQuery` (`Offset`, `Limit`, `Sort[]`, `Filter[]`), validate through
`QueryParams.Create`, then apply with `Query<T>`. Never pass raw offset/limit down the stack.

```csharp
public record GetOrdersQuery(int Offset, int Limit, string[] Sort, string[] Filter) : IRequestQuery;

public async Task<Result<QueryResult<OrderDto>>> HandleAsync(GetOrdersQuery query, CancellationToken ct)
{
    var paramsResult = QueryParams.Create(query);
    if (paramsResult.IsFailure) return paramsResult.Error;

    var queryable = _dbContext.Orders.AsNoTracking();
    var total = await queryable.CountAsync(ct);
    var items = await queryable.Query(paramsResult.Value).ToListAsync(ct);

    return new QueryResult<OrderDto>(total, items.Select(MapToDto));
}
```

Bind the query string with `HttpContext.ToQueryRequest`:

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
});
```

`Query<T>` translates the string sort/filter with `System.Linq.Dynamic.Core`, so index
every filterable and sortable column, and whitelist which column names a request accepts.

---

## Endpoint filters

Cross-cutting concerns attach per endpoint, validation before unit of work so an invalid
request never opens a transaction:

```csharp
app.MapPost("/orders", CreateOrder)
    .AddEndpointFilter<ValidationFilter<CreateOrderCommand>>()   // outermost
    .AddEndpointFilter<UnitOfWorkFilter>()
    .RequireAuthorization("CanManageOrders");
```

| Filter | Effect |
|---|---|
| `ValidationFilter<TRequest>` | Resolves `IValidator<TRequest>`; on failure short-circuits with `ValidationProblem` and never calls the endpoint. No-op when no validator is registered |
| `UnitOfWorkFilter` | Resolves `IUnitOfWork`; `BeginWorkAsync` → endpoint → `CommitWorkAsync`, with `RollbackWorkAsync` and a rethrow on any exception |

`QueryValidatorRoot<TRequest>` already validates `Offset`/`Limit`/`Sort`/`Filter` —
subclasses must not restate those rules.

The transaction spans the whole endpoint, so keep HTTP calls, file I/O and other slow
non-transactional work outside it (outbox or saga instead). Read endpoints take neither
filter.

Both are plain `IEndpointFilter` implementations — add your own alongside them.

## Repositories and unit of work

`IRepository<TAggregate>` exposes `SaveAsync` only — writes go through it, reads do not.
Read with `DbContext`, Dapper or a dedicated query service straight from the query handler.
`IUnitOfWork` is `BeginWorkAsync` / `CommitWorkAsync` / `RollbackWorkAsync`, driven by
`UnitOfWorkFilter` rather than by hand.

---

## Authorization composition

Compose existing policies instead of duplicating their logic:

```csharp
options.AddPolicy("AdminModerator", policy =>
    policy.AddRequirements(MultiplePoliciesRequirement.ForAllOf("Admin", "Moderator")));

options.AddPolicy("AdminOrModerator", policy =>
    policy.AddRequirements(MultiplePoliciesRequirement.ForAnyOf("Admin", "Moderator")));

services.AddSingleton<IAuthorizationHandler, MultiplePoliciesHandler>();
```

`services.AddUserContext()` registers `IHttpUserContext` for the current caller's identity.

---

## Background work

```csharp
services.AddHangfireRequestScheduler();   // registers IRequestScheduler

_scheduler.QueueHandler<IOrderProcessor>(
    handler => handler.ProcessAsync(orderId),   // pass IDs, never whole entities
    "Process order");
```

Job arguments are JSON-serialized into the Hangfire store — keep them small.

---

## Testing

```csharp
// Fake builders — one per aggregate
public class FakeOrderBuilder : FakerBuilder<Order>
{
    protected override Faker<Order> Faker => CreateUninitializedFaker<Order>()
        .RuleFor(o => o.Id, _ => OrderId.New)
        .RuleFor(o => o.Total, f => f.Finance.Amount(1, 10_000));

    public FakeOrderBuilder WithCustomer(CustomerId id)
    {
        Faker.RuleFor(o => o.CustomerId, _ => id);
        return this;
    }
}

var order = new FakeOrderBuilder().Build();
var orders = new FakeOrderBuilder().BuildCollection();          // random count
var saved = await new FakeOrderBuilder().SavedInDbAsync(_databaseDriver);
```

`CreateFaker<T>()` for public setters, `CreatePrivateFaker<T>()` for private ones,
`CreateUninitializedFaker<T>()` when the type has no public constructor.

Other drivers: `DatabaseDriver<TDbContext>` (Testcontainers MariaDB/PostgreSQL + Respawn,
`AddDatabaseDriver<TDbContext>` to register), `WireMockDriver` for outbound HTTP,
`AuthorityDriver` for a full OpenID Connect authority including JWKS, `JwtTokenBuilder`
for tokens, `FeatureManagerDriver` with `[FeatureFlagTest]` for feature flags,
`ApiResponseAssertions` for Refit `IApiResponse` assertions.

`DatabaseDriver<TDbContext>` implements `IAsyncLifetime` — let xUnit call
`InitializeAsync`, never invoke it yourself. Point the test host's authority
configuration at `AuthorityDriver`'s URL before the app starts.

Categorize tests with `[UnitTest]`, `[ComponentTest]` or `[EndToEndTest]`.

---

## Rules

**DO:**
- Give every aggregate a `StronglyTypedId<T>`, and register both converters
- Return `Result<T>` from anything that can fail; define errors as static members near their type
- Validate paging through `QueryParams.Create` and apply it with `Query<T>`
- Put `ValidationFilter<TRequest>` before `UnitOfWorkFilter` on mutating endpoints
- Leave both filters off read endpoints
- Keep NuGet versions in `Directory.Packages.props` (central package management)
- `sealed` by default, `record` for IDs and value objects, async all the way down

**DO NOT:**
- Use a raw `Guid` as entity identity
- Throw for expected domain outcomes, or return `null` from domain methods
- Read `result.Value` without checking `IsSuccess`
- Construct a `ResultError` without both `Code` and `Description`
- Pass unvalidated `offset`/`limit`/`sort`/`filter` strings into a query
- Call `IRepository<T>` for reads — it only has `SaveAsync`
- Open a transaction before validating — `UnitOfWorkFilter` never goes first
- Do slow or external work inside an endpoint wrapped in `UnitOfWorkFilter`
- Put a `Version` attribute on a `<PackageReference>`

---

## Further reading

Full documentation ships inside every package and lives in this repository:
`docs/API.md` (every signature), `docs/EXAMPLES.md` (end-to-end patterns),
`docs/AI-GUIDELINES.md` (patterns, anti-patterns, performance, gotchas),
`ARCHITECTURE.md` (package map and design decisions).
