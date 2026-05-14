# Architecture — DrifterApps Seeds

A collection of opinionated utility libraries for building DDD/ASP.NET Core applications following Domain Driven Design, Vertical Slice Architecture, and Cloud Design Patterns.

## Package Map

```
DrifterApps.Seeds.Domain          (no seed dependencies)
    ↑
DrifterApps.Seeds.Application     → Domain
    ↑
DrifterApps.Seeds.Application.Mediatr → Application, Domain
    ↑
DrifterApps.Seeds.Infrastructure  → Application

DrifterApps.Seeds.Testing         → Domain, Infrastructure
```

Each package is an independent NuGet library. Install only what your layer needs.

---

## Packages

### Domain

**Purpose:** Core DDD building blocks with no infrastructure dependencies.

| Type | Role |
|---|---|
| `IAggregateRoot` | Marker for aggregate roots |
| `IAggregateRoot<T>` | Generic version exposing a strongly-typed `Id` |
| `IRepository<TAggregate>` | Single-method save contract — no query methods |
| `IUnitOfWork` | Transaction boundary; `BeginWork` / `CommitWork` / `RollbackWork` |
| `StronglyTypedId<T>` | GUID-backed identity value type for aggregates |
| `IPrimitiveType<T>` | Base interface for all primitive value wrappers |

**Key decision — `IRepository<T>` is write-only.** Queries are intentionally out of scope for the repository contract; they belong in query handlers that reach the read model directly (Dapper, raw EF queries, etc.). This avoids the "fat repository" anti-pattern.

### Application

**Purpose:** ASP.NET Core plumbing — pagination, authorization, JSON serialization, and EF Core support for domain types.

| Type / Extension | Role |
|---|---|
| `QueryParams` | Validated pagination/sort/filter value object |
| `IRequestQuery` | Contract for HTTP query strings that drive `QueryParams` |
| `QueryResult<T>` | Paginated response envelope `(Total, Items)` |
| `QueryableExtensions.Query<T>` | Applies `QueryParams` to any `IQueryable<T>` |
| `MultiplePoliciesRequirement` | AND/OR composition of ASP.NET Core authorization policies |
| `MultiplePoliciesHandler` | Evaluates `MultiplePoliciesRequirement` against the DI container |
| `StronglyTypedIdJsonConverterFactory` | System.Text.Json converter for `StronglyTypedId<T>` |
| `StronglyTypedIdValueConverter<T>` | EF Core value converter: `StronglyTypedId<T>` ↔ `Guid` |
| `ValidationFilter<TRequest>` | Minimal API endpoint filter — validates with FluentValidation |
| `UnitOfWorkFilter` | Minimal API endpoint filter — commits `IUnitOfWork` after handler |
| `HttpContextExtensions.ToQueryRequest<T>` | Parses HTTP query string into `IRequestQuery` |
| `ValidatorExtensions.IsValidAsync<T>` | Runs FluentValidation and returns `Result<T>` |
| `QueryValidatorRoot<T>` | Base FluentValidation validator with offset/limit rules pre-wired |

**Key decision — `QueryParams` is a validated value object, not a raw DTO.** `Create()` returns `Result<QueryParams>` and enforces sort/filter syntax at the boundary. Downstream code can assume the params are structurally valid.

**Filter syntax:** `property:operator:value`
- Operators: `eq`, `ne`, `lt`, `le`, `gt`, `ge`
- Example: `name:eq:Alice`, `age:gt:30`

**Sort syntax:** `field` (ascending) or `-field` (descending)
- Example: `name`, `-createdAt`

### Application.Mediatr

**Purpose:** MediatR pipeline behaviors for cross-cutting concerns.

| Behavior | Trigger | Effect |
|---|---|---|
| `LoggingBehavior<TReq, TRes>` | All `IRequest<TResponse>` | Structured log at start and end |
| `ValidationBehavior<TReq, TRes>` | All requests with registered validators | Returns `Result<T>` failure or throws `ValidationException` |
| `UnitOfWorkBehavior<TReq, TRes>` | Requests implementing `IUnitOfWorkRequest` | Wraps handler with `BeginWork` → handler → `CommitWork`; rolls back on exception |

Registration order matters: `LoggingBehavior` runs first (outermost), then `UnitOfWorkBehavior`, then `ValidationBehavior`. `RegisterServicesFromApplicationSeeds()` sets this order automatically — call it before adding your own behaviors.

**Key decision — validation before unit-of-work commit.** If the request fails validation, no transaction is opened. If the handler fails at runtime, the unit of work is rolled back.

### Infrastructure

**Purpose:** Concrete implementations of application contracts backed by third-party infrastructure.

| Type | Implements | Backing library |
|---|---|---|
| `RequestScheduler` | `IRequestScheduler` | Hangfire |
| `RefitExtensions.ToProblemDetails` | — | Refit |

`AddHangfireRequestScheduler(services)` registers `IRequestScheduler`. Pass an optional `Func<JsonSerializerOptions>` to control how job arguments are serialized.

### Testing

**Purpose:** Reusable test infrastructure. Install only in test projects.

| Type | Role |
|---|---|
| `FakerBuilder<T>` | Bogus-backed builder; produces single or collection of fake objects |
| `DatabaseDriver<TDbContext>` | Manages container lifecycle (Testcontainers) + state reset (Respawn) |
| `WireMockDriver` | Base for WireMock.Net server wrappers |
| `AuthorityDriver` | Mock OpenID Connect authority (JWKS + `.well-known`) |
| `JwtTokenBuilder` | Fluent builder for signed JWT tokens using the mock authority's key |
| `[UnitTest]`, `[ComponentTest]`, `[EndToEndTest]` | xUnit trait attributes for test categorization |
| `ApiResponseAssertions<T>` | FluentAssertions extension for `IApiResponse<T>` from Refit |
| `StronglyTypedIdEquivalencyStep` | FluentAssertions equivalency step for `StronglyTypedId<T>` |

---

## Core Design Decisions

### Result pattern, not exceptions

All fallible operations return `Result<T>` (from `DrifterApps.Seeds.FluentResult`) rather than throwing. Exceptions are reserved for programming errors (null arguments, broken invariants). The `ValidationBehavior` maps FluentValidation failures to `Result<T>` before the handler runs.

### Strongly-typed identifiers

Raw `Guid` is never used as an entity ID in public APIs. `StronglyTypedId<T>` prevents passing a `CustomerId` where an `OrderId` is expected — the compiler catches it. The base record provides full `IComparable<T>`, `IEqualityComparer<T>`, `IParsable<T>`, and implicit `Guid`/`string` conversion so downstream code stays ergonomic.

### Central package management

All NuGet version pins live in `Directory.Packages.props`. Individual `.csproj` files never specify versions. This prevents version drift across projects and makes upgrades a single-line change.

### Warnings as errors

`TreatWarningsAsErrors=true` and `EnforceCodeStyleInBuild=true` apply everywhere. Nullable reference types are enabled. The CI linter (`dotnet format --severity error`) enforces formatting; PRs fail on violations.

### No fat repositories

`IRepository<TAggregate>` has one method: `SaveAsync`. Read models query the data store directly through query handlers — using EF Core, Dapper, or whatever fits. This keeps read and write paths independent.

---

## Extension Points

| What to extend | How |
|---|---|
| Add a MediatR behavior | Implement `IPipelineBehavior<TRequest, TResponse>` and register via `AddOpenBehavior` after `RegisterServicesFromApplicationSeeds()` |
| Custom authorization policy | Implement `IAuthorizationRequirement` + `AuthorizationHandler<T>`; compose with `MultiplePoliciesRequirement` |
| New database server for testing | Implement `IDatabaseServer`; pass to `DatabaseDriver<TDbContext>` |
| Custom Bogus fake | Subclass `FakerBuilder<T>`; override `Faker` property |
| Custom strongly-typed ID | `public record MyId : StronglyTypedId<MyId>;` — nothing else needed |
