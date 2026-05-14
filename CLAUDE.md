# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

This is a collection of NuGet utility libraries for building DDD/ASP.NET applications, organized around Domain Driven Design, Vertical Slice Architecture, and Cloud Design Patterns. All packages are published to NuGet under the `DrifterApps.Seeds.*` namespace.

## Commands

### Build & Test

```bash
# Build
dotnet build --configuration Release

# Run all tests
dotnet test --configuration Release

# Run a single test project
dotnet test tests/Domain.Tests --configuration Release
dotnet test tests/Application.Tests --configuration Release
dotnet test tests/Infrastructure.Tests --configuration Release
dotnet test tests/Testing.Tests --configuration Release

# Run with coverage
dotnet test --configuration Release --collect 'XPlat Code Coverage'

# Lint / format check (CI uses this)
dotnet format --severity error
```

### Local NuGet packaging

`build.sh` auto-increments a local `version.txt` counter, packs with version `1.0.{counter}-alpha`, and pushes to `~/.nuget/local-packages/`.

```bash
./build.sh
```

## Architecture

### Project dependency graph

```
Domain (no deps on other seed projects)
  ↑
Application → Domain
  ↑
Application.Mediatr → Application, Domain
  ↑
Infrastructure → Application

Testing → Domain, Infrastructure
```

### Source projects

- **Domain** — Core DDD interfaces: `IAggregateRoot`, `IDomainPublisher`, `IRepository<T>`, `IUnitOfWork`, `DomainException`/`DomainException<TContext>`
- **Application** — Application layer utilities: authorization policies/filters, endpoint filters, entity converters, DI extensions for ASP.NET Core
- **Application.Mediatr** — MediatR pipeline behaviors: `ValidationBehavior`, `LoggingBehavior`, `UnitOfWorkBehavior`; FluentValidation error mapping
- **Infrastructure** — Hangfire-backed `IRequestScheduler`, Refit HTTP client support
- **Testing** — Shared test infrastructure: `FakerBuilder` (Bogus), `DatabaseDriver` (Testcontainers + Respawn), `WireMockDriver`, `ScenarioRunner`, custom FluentAssertions extensions, `FeatureFlagTestAttribute`

### Key design decisions

- **Central package management** — all package versions are in `Directory.Packages.props`; `.csproj` files must not specify versions
- **Warnings as errors** — `TreatWarningsAsErrors=true` globally; all analyzer warnings are errors; `EnforceCodeStyleInBuild=true`
- **Nullable enabled** — everywhere; handle nullability explicitly
- **`InternalsVisibleTo`** — each project automatically exposes internals to `<AssemblyName>.Tests` and `DynamicProxyGenAssembly2` (NSubstitute)
- **Result pattern** — `DrifterApps.Seeds.FluentResult` is used instead of exceptions for operation outcomes
- **Scenario testing** — `DrifterApps.Seeds.FluentScenario` enables BDD-style scenario runners in tests

### Test stack

xUnit v3 · FluentAssertions · NSubstitute · Bogus · Testcontainers (MariaDb + PostgreSQL) · Respawn · WireMock.Net · coverlet

## CI/CD

- **ci-cd.yml** — build → test with coverage → publish NuGet on release; uses GitVersion for semver
- **linter.yml** — runs `dotnet format --severity error`; PRs fail on formatting violations
- **codeql-analysis.yml** — CodeQL security scanning on main and PRs

Version bumps via commit message: `+semver: breaking`/`major`, `+semver: feature`/`minor`, `+semver: fix`/`patch`
