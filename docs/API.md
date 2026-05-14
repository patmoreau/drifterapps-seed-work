# API Reference — DrifterApps Seeds

Full public API reference for all `DrifterApps.Seeds.*` packages. Target framework: **.NET 10**.

---

## DrifterApps.Seeds.Domain

NuGet: `DrifterApps.Seeds.Domain`  
Namespace: `DrifterApps.Seeds.Domain`

### `IAggregateRoot`

Marker interface. Apply to every aggregate root class.

```csharp
public interface IAggregateRoot { }
```

### `IAggregateRoot<out T>`

Generic variant that exposes a strongly-typed `Id` property.

```csharp
public interface IAggregateRoot<out T> : IAggregateRoot
    where T : IStronglyTypedId
{
    T Id { get; }
}
```

### `IRepository<in TAggregate>`

Write-only repository contract.

```csharp
public interface IRepository<in TAggregate>
    where TAggregate : IAggregateRoot
{
    Task SaveAsync(TAggregate aggregate, CancellationToken cancellationToken = default);
}
```

### `IUnitOfWork`

Transaction boundary contract.

```csharp
public interface IUnitOfWork : IAsyncDisposable
{
    Task BeginWork(CancellationToken cancellationToken = default);
    Task CommitWork(CancellationToken cancellationToken = default);
    Task RollbackWork(CancellationToken cancellationToken = default);
}
```

### `IPrimitiveType<out T>`

Base interface for primitive value wrappers.

```csharp
public interface IPrimitiveType<out T>
{
    T Value { get; }
}
```

### `IStronglyTypedId`

Marker interface for strongly-typed identifier types.

```csharp
public interface IStronglyTypedId : IPrimitiveType<Guid> { }
```

### `StronglyTypedId<T>`

Abstract base record for GUID-backed strongly-typed identifiers. Inherit with a single-line declaration.

```csharp
public abstract record StronglyTypedId<T> : IStronglyTypedId,
    IEqualityComparer<T>, IComparable<T>, IParsable<T>
    where T : StronglyTypedId<T>, new()
```

#### Static members

| Member | Type | Description |
|---|---|---|
| `New` | `T` | New instance with a freshly generated `Guid` |
| `Empty` | `T` | Instance with `Guid.Empty` |
| `Create(Guid value)` | `T` | Factory from an existing `Guid` |
| `Parse(string s, IFormatProvider? provider)` | `T` | Parses GUID string; returns `Empty` if invalid |
| `TryParse(string? s, IFormatProvider? provider, out T result)` | `bool` | Returns `false` and `Empty` on failure |

#### Instance members

| Member | Type | Description |
|---|---|---|
| `Value` | `Guid` | Underlying GUID |
| `CompareTo(T? other)` | `int` | Implements `IComparable<T>` |
| `Equals(T? other)` | `bool` | Value equality |
| `GetHashCode()` | `int` | Based on `Value` |
| `Equals(T? x, T? y)` | `bool` | `IEqualityComparer<T>` implementation |
| `GetHashCode(T obj)` | `int` | `IEqualityComparer<T>` implementation |

#### Operators

| Operator | Description |
|---|---|
| `implicit operator Guid` | `StronglyTypedId<T>` → `Guid` |
| `implicit operator StronglyTypedId<T>(Guid)` | `Guid` → `T` |
| `implicit operator string` | `StronglyTypedId<T>` → GUID string |
| `implicit operator StronglyTypedId<T>(string)` | Parses GUID string → `T`; returns `Empty` if invalid |
| `>`, `<`, `>=`, `<=` | Comparison operators delegating to `CompareTo` |

---

## DrifterApps.Seeds.Application

NuGet: `DrifterApps.Seeds.Application`  
Root namespace: `DrifterApps.Seeds.Application`

### `IRequestQuery`

Contract for HTTP query string DTOs passed to query handlers.

```csharp
public interface IRequestQuery
{
    int Offset { get; }
    int Limit { get; }
    string[] Sort { get; }
    string[] Filter { get; }
}
```

### `IRequestScheduler`

```csharp
public interface IRequestScheduler
{
    string QueueHandler<THandler>(
        Expression<Func<THandler, Task>> methodCall,
        string description);
}
```

### `IHttpUserContext`

```csharp
public interface IHttpUserContext
{
    string IdentityObjectId { get; }
}
```

Resolved from `NameIdentifier` or `"sub"` claim. Register via `services.AddUserContext()`.

### `QueryParams`

Immutable, validated query parameters value type.

```csharp
public readonly partial struct QueryParams : IEquatable<QueryParams>
```

#### Constants / defaults

| Member | Value |
|---|---|
| `DefaultOffset` | `0` |
| `DefaultLimit` | `int.MaxValue` |
| `DefaultSort` | `[]` |
| `DefaultFilter` | `[]` |
| `Empty` | `{ Offset=0, Limit=int.MaxValue, Sort=[], Filter=[] }` |

#### Properties

| Property | Type |
|---|---|
| `Offset` | `int` |
| `Limit` | `int` |
| `Sort` | `IReadOnlyCollection<string>` |
| `Filter` | `IReadOnlyCollection<string>` |

#### Factory methods

```csharp
static Result<QueryParams> Create(IRequestQuery requestQuery)
static Result<QueryParams> Create(int offset, int limit,
    IReadOnlyCollection<string> sort,
    IReadOnlyCollection<string> filter)
```

**Validation rules:**
- `offset >= 0` → `QueryParamsErrors.OffsetCannotBeNegative`
- `limit > 0` → `QueryParamsErrors.LimitMustBePositive`
- Each sort entry must match `^(?<desc>-?)(?<field>\w+)$`
- Each filter entry must match `^(?<property>\w+)(?<operator>:(eq|ne|lt|le|gt|ge):)(?<value>.+)$`

### `QueryResult<TResult>`

```csharp
public record QueryResult<TResult>(int Total, IEnumerable<TResult> Items);
```

### `QueryValidatorRoot<TRequest>`

Abstract FluentValidation base validator. Pre-wires `Offset >= 0` and `Limit > 0` rules.

```csharp
public abstract class QueryValidatorRoot<TRequest> : AbstractValidator<TRequest>
    where TRequest : IRequestQuery
```

### `QueryParamsErrors` (static)

| Member | Type |
|---|---|
| `RequestIsRequired` | `ResultError` |
| `OffsetCannotBeNegative` | `ResultError` |
| `LimitMustBePositive` | `ResultError` |
| `SortInvalidPattern(string sort)` | `ResultError` |
| `FilterInvalidPattern(string filter)` | `ResultError` |

---

### Namespace: `DrifterApps.Seeds.Application.Authorization`

#### `MultiplePoliciesRequirement`

```csharp
public sealed class MultiplePoliciesRequirement : IAuthorizationRequirement
{
    IReadOnlyCollection<string> Policies { get; }
    bool All { get; }   // true = ForAllOf, false = ForAnyOf

    static MultiplePoliciesRequirement ForAllOf(params string[] policies);
    static MultiplePoliciesRequirement ForAnyOf(params string[] policies);
}
```

#### `MultiplePoliciesHandler`

```csharp
public class MultiplePoliciesHandler(IServiceProvider serviceProvider)
    : AuthorizationHandler<MultiplePoliciesRequirement>
```

Register as `services.AddSingleton<IAuthorizationHandler, MultiplePoliciesHandler>()`.

---

### Namespace: `DrifterApps.Seeds.Application.Converters`

#### `StronglyTypedIdJsonConverterFactory`

```csharp
public class StronglyTypedIdJsonConverterFactory : JsonConverterFactory
{
    bool CanConvert(Type typeToConvert);
    JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options);
}
```

Handles any `StronglyTypedId<T>` subtype. Serializes to/from the GUID string representation.

#### `StronglyTypedIdJsonConverter<T>`

```csharp
public class StronglyTypedIdJsonConverter<T> : JsonConverter<StronglyTypedId<T>>
    where T : StronglyTypedId<T>, new()
{
    StronglyTypedId<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options);
    void Write(Utf8JsonWriter writer, StronglyTypedId<T> value, JsonSerializerOptions options);
}
```

---

### Namespace: `DrifterApps.Seeds.Application.Context`

#### `StronglyTypedIdValueConverter<TStronglyTypedId>`

```csharp
public class StronglyTypedIdValueConverter<TStronglyTypedId>
    : ValueConverter<TStronglyTypedId, Guid>
    where TStronglyTypedId : StronglyTypedId<TStronglyTypedId>, new()
```

Use in `OnModelCreating`:

```csharp
builder.Property(x => x.Id)
    .HasConversion(new StronglyTypedIdValueConverter<OrderId>());
```

---

### Namespace: `DrifterApps.Seeds.Application.EndpointFilters`

#### `ValidationFilter<TRequest>`

Validates the first argument of an endpoint that matches `TRequest` using all registered `IValidator<TRequest>` instances. Returns `ValidationProblemDetails` (HTTP 422) on failure.

```csharp
public class ValidationFilter<TRequest> : IEndpointFilter
{
    ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next);
}
```

#### `UnitOfWorkFilter`

Calls `IUnitOfWork.CommitWork()` after the endpoint handler returns successfully. Register after `ValidationFilter` in the filter chain.

```csharp
public class UnitOfWorkFilter : IEndpointFilter
{
    ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next);
}
```

---

### Namespace: `DrifterApps.Seeds.Application.Extensions`

#### `HttpContextExtensions`

```csharp
public static class HttpContextExtensions
{
    static ValueTask<TRequest?> ToQueryRequest<TRequest>(
        this HttpContext context,
        RequestQueryFactory<TRequest> requestQueryFactory)
        where TRequest : IRequestQuery;
}

public delegate TRequest? RequestQueryFactory<out TRequest>(
    int offset, int limit, string[] sort, string[] filter);
```

Parses `offset`, `limit`, `sort`, `filter` from the HTTP query string and calls the factory.

#### `QueryableExtensions`

```csharp
public static class QueryableExtensions
{
    static IQueryable<T> Query<T>(this IQueryable<T> query, QueryParams queryParams);
}
```

Applies filters, sorts, and pagination (skip/take) in that order. Filter and sort expressions are translated by `System.Linq.Dynamic.Core`.

#### `ResultErrorExtensions`

```csharp
public static class ResultErrorExtensions
{
    static IResult ToProblemDetails(this ResultError error, int statusCode = 400);
    static IResult ToValidationProblemDetails(this ResultErrorAggregate error);
}
```

#### `ValidatorExtensions`

```csharp
public static class ValidatorExtensions
{
    static Task<Result<T>> IsValidAsync<T>(
        this IValidator<T> validator,
        T instance,
        CancellationToken cancellationToken = default);
}
```

Returns `Result<T>.Success(instance)` if valid; otherwise a `ResultErrorAggregate` failure.

#### `ServiceCollectionExtensions`

```csharp
public static class ServiceCollectionExtensions
{
    static IServiceCollection AddUserContext(this IServiceCollection services);
}
```

Registers `HttpUserContext` as `IHttpUserContext` (scoped). Requires `IHttpContextAccessor`.

---

## DrifterApps.Seeds.Application.Mediatr

NuGet: `DrifterApps.Seeds.Application.Mediatr`  
Namespace: `DrifterApps.Seeds.Application.Mediatr`

### `IUnitOfWorkRequest`

Marker interface. Attach to MediatR request records that mutate state and need a transaction.

```csharp
public interface IUnitOfWorkRequest { }
```

### `LoggingBehavior<TRequest, TResponse>`

```csharp
public sealed class LoggingBehavior<TRequest, TResponse>(ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
```

Logs at `Information` level before and after the handler. First behavior in the pipeline (outermost).

### `UnitOfWorkBehavior<TRequest, TResponse>`

```csharp
public sealed class UnitOfWorkBehavior<TRequest, TResponse>(IUnitOfWork unitOfWork)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IUnitOfWorkRequest, IBaseRequest
```

Wraps the handler: `BeginWork` → handler → `CommitWork`. Calls `RollbackWork` on any exception and re-throws.

### `ValidationBehavior<TRequest, TResponse>`

```csharp
public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseRequest
```

- If `TResponse` is `Result<T>`: returns a `ResultErrorAggregate` failure.
- Otherwise: throws `ValidationException`.

### `MediatRServiceConfigurationExtensions`

```csharp
public static class MediatRServiceConfigurationExtensions
{
    static MediatRServiceConfiguration RegisterServicesFromApplicationSeeds(
        this MediatRServiceConfiguration config);
}
```

Registers `LoggingBehavior`, `UnitOfWorkBehavior`, `ValidationBehavior` (in that pipeline order) plus all services from the `Application.Mediatr` assembly. Call before any other behavior registrations.

---

## DrifterApps.Seeds.Infrastructure

NuGet: `DrifterApps.Seeds.Infrastructure`  
Namespace: `DrifterApps.Seeds.Infrastructure`

### `RefitExtensions`

```csharp
public static class RefitExtensions
{
    static Task<ProblemDetails?> ToProblemDetails(this ApiException exception);
    static Task<ValidationProblemDetails?> ToValidationProblemDetails(this ApiException exception);
}
```

Deserializes a Refit `ApiException` body into ASP.NET Core `ProblemDetails`.

### `ServiceCollectionExtensions`

```csharp
public static class ServiceCollectionExtensions
{
    static IServiceCollection AddHangfireRequestScheduler(
        this IServiceCollection services,
        Func<JsonSerializerOptions>? jsonSerializerOptionsFactory = null);
}
```

Registers `RequestScheduler` as `IRequestScheduler`. Optionally supply custom JSON serialization options for Hangfire job arguments.

---

## DrifterApps.Seeds.Testing

NuGet: `DrifterApps.Seeds.Testing`  
Namespace: `DrifterApps.Seeds.Testing`

### `IDriverOf<out TSystemUnderTest>`

```csharp
public interface IDriverOf<out TSystemUnderTest>
{
    TSystemUnderTest Build();
}
```

### `FakerBuilder<TFaked>`

Abstract Bogus-backed builder. Subclass and override `Faker`.

```csharp
public abstract partial class FakerBuilder<TFaked> where TFaked : class
{
    protected abstract Faker<TFaked> Faker { get; }

    TFaked Build();
    IReadOnlyCollection<TFaked> BuildCollection(int? count = null);

    // Factory helpers (protected)
    protected static Faker<T> CreateFaker<T>() where T : class;
    protected static Faker<T> CreateUninitializedFaker<T>() where T : class;
}
```

`CreateUninitializedFaker<T>()` creates objects without calling constructors — useful for `record` types and classes without parameterless constructors.

### `DatabaseDriver<TDbContext>`

Abstract Testcontainers + Respawn database driver.

```csharp
public abstract partial class DatabaseDriver<TDbContext> : IAsyncLifetime
    where TDbContext : DbContext
{
    protected abstract IDatabaseServer DatabaseServer { get; init; }

    virtual ValueTask InitializeAsync();
    virtual ValueTask DisposeAsync();
}
```

Implement `IDatabaseServer` with one of the built-ins:

| Implementation | Container |
|---|---|
| `LocalDatabaseServer` | No container — local connection string |
| `PostgreDatabaseServer(PostgreSqlContainer)` | PostgreSQL via Testcontainers |
| `MariaDatabaseServer(MariaDbContainer)` | MariaDB via Testcontainers |

### `WireMockDriver`

Abstract WireMock.Net server wrapper.

```csharp
public abstract class WireMockDriver : IAsyncLifetime
{
    protected WireMockServer Server { get; }

    virtual ValueTask InitializeAsync();
    virtual ValueTask DisposeAsync();
    virtual WireMockServer CreateServer();
    abstract void Configure();
}
```

### `AuthorityDriver`

Pre-built OpenID Connect authority mock for integration tests.

```csharp
public sealed class AuthorityDriver : WireMockDriver
{
    Uri Authority { get; }
}
```

Exposes a `/.well-known/openid-configuration` endpoint and a JWKS endpoint. Use `JwtTokenBuilder` to generate tokens signed by the matching key.

### `JwtTokenBuilder`

Fluent builder for signed JWT tokens.

```csharp
public sealed class JwtTokenBuilder
{
    JwtTokenBuilder ForAudience(string audience);
    JwtTokenBuilder IssuedBy(string issuer);
    JwtTokenBuilder ValidOn(DateTime notBefore);
    JwtTokenBuilder ExpireIn(TimeSpan expiry);
    JwtTokenBuilder WithClaim(string type, string value);
    JwtTokenBuilder WithRole(string role);
    JwtTokenBuilder WithScopes(params string[] scopes);
    string Build();
}
```

### `JwtSigningCredentials`

```csharp
public sealed class JwtSigningCredentials
{
    static SigningCredentials SigningCredentials { get; }
    static SigningKeyInfo GetSigningKeyInfo { get; }
}

public record SigningKeyInfo(string Modulus, string Exponent, string Kid, string Algorithm);
```

### Test category attributes

| Attribute | xUnit trait |
|---|---|
| `[UnitTest]` | `Category=Unit` |
| `[ComponentTest]` | `Category=Component` |
| `[EndToEndTest]` | `Category=EndToEnd` |
| `[FeatureFlagTest]` | `Category=FeatureFlag` |

### `ApiResponseAssertions<TValue>`

FluentAssertions extension for `IApiResponse<TValue>` (Refit).

```csharp
public class ApiResponseAssertions<TValue>
{
    AndConstraint<ApiResponseAssertions<TValue>> HaveContent(
        string because = "", params object[] becauseArgs);

    AndConstraint<ApiResponseAssertions<TValue>> HaveContent(
        TValue expectedValue,
        string because = "", params object[] becauseArgs);

    AndConstraint<ApiResponseAssertions<TValue>> HaveEquivalentContent(
        TValue expectedValue,
        Func<EquivalencyAssertionOptions<TValue>,
            EquivalencyAssertionOptions<TValue>>? config = null,
        string because = "", params object[] becauseArgs);
}
```

### `ServiceCollectionExtensions` (Testing)

```csharp
public static class ServiceCollectionExtensions
{
    static IServiceCollection AddDatabaseDriver<TDbContext>(
        this IServiceCollection services,
        DatabaseDriver<TDbContext> driver)
        where TDbContext : DbContext;
}
```

Replaces existing `DbContext` registrations with the driver's options. Adds `DbContextOptions<TDbContext>` and `IDbContextFactory<TDbContext>`.
