# DrifterApps Seeds — Agent Instructions

## Engineering principles
@docs/contributing/engineering-principles.md

## Git workflow
@docs/contributing/git-instructions.md

## Task playbooks

Recurring procedures live in `docs/contributing/tasks/`, one file per task. Read
the matching playbook before starting that work:

| Task | When |
|---|---|
| `add-seed-utility.md` | Adding or changing a public type in any `src/` package |
| `test-package-locally.md` | Proving a change from a consuming solution via `build.sh` |
| `create-github-release.md` | Cutting a release, which publishes to nuget.org |

`scripts/sync-agent-files.sh` copies each playbook into the per-assistant command
formats — `.claude/commands/`, `.cursor/commands/`, `.windsurf/workflows/` and
`.github/prompts/` — so the same procedure is invocable as `/<task-name>` in each
tool. Those copies are **generated**: edit the file in `docs/contributing/tasks/`,
rerun the script, and commit both. `scripts/sync-agent-files.sh --check` runs in
the `linter` workflow and fails when a copy is stale or orphaned.

Note the distinction from the rules files this repo *ships* for consumers
(`.cursor/rules/drifterapps-seeds.mdc`, `.windsurf/rules/drifterapps-seeds.md`,
`.junie/guidelines.md`, `.github/copilot-instructions.md`, `llms.txt`) and the
published documentation they point at (`README.md`, `ARCHITECTURE.md`, `docs/API.md`,
`docs/EXAMPLES.md`, `docs/AI-GUIDELINES.md`): those are hand-maintained and describe
how to *use* the packages. The generated command files and everything under
`docs/contributing/` describe how to *work on* them, and never ship.

When the public surface changes, the consumer rules files change with it — they
restate the API, so a stale one teaches assistants a signature that no longer exists.

## What this repository is

This repo **is** the source of the `DrifterApps.Seeds.*` NuGet packages —
opinionated building blocks for DDD / Vertical Slice Architecture ASP.NET Core
applications. You are working *on* the libraries, not merely *with* them.

| Path | Contents |
|---|---|
| `src/Domain/` | DDD contracts: `IAggregateRoot`, `IAggregateRoot<T>`, `IRepository<T>`, `IUnitOfWork`, `IStronglyTypedId`/`StronglyTypedId`, `IPrimitiveType` |
| `src/Application/` | ASP.NET Core application layer: `Authorization/`, `EndpointFilters/`, `Converters/` (EF Core + JSON), `Context/`, `QueryParams`/`QueryResult`, `IRequestScheduler`, `IHttpUserContext`, DI extensions |
| `src/Infrastructure/` | Hangfire-backed `RequestScheduler`, `RefitExtensions`, `JsonSerializerOptionsFactory`, DI extensions |
| `src/Testing/` | Test infrastructure for consumers: `FakerBuilder`, `DatabaseDriver` (Testcontainers + Respawn), `WireMockDriver`, drivers, FluentAssertions extensions, trait attributes (`[UnitTest]`, `[ComponentTest]`, `[EndToEndTest]`, `FeatureFlagTestAttribute`) |
| `tests/*.Tests/` | xUnit v3 suites, one per source project |
| `examples/` | Illustrative sources (`Domain`, `Application`, `Testing`) — **not a compiled project**, no csproj |
| `README.md`, `ARCHITECTURE.md`, `docs/` | Published documentation, for people **using** the packages — packed into every NuGet package |
| `docs/contributing/` | Process docs, for people **working on** this repo — never packed |

Package ids are `DrifterApps.Seeds.<Project>`; namespaces match. Target is
`net10.0` only — the root `Directory.Build.props` owns `TargetFramework`, and
projects must not override it. SDK pinned in `global.json` (10.0.400,
`rollForward: latestMinor`).

Package dependency graph — one-directional, keep it that way:

```
Domain  ←  Application  ←  Infrastructure
Testing  →  Domain, Infrastructure
```

MediatR support is gone: `src/Application.Mediatr` was removed (it had been orphaned
from the solution since `3c56a3c`, unbuilt and unpublished since 1.0.150). The package
`DrifterApps.Seeds.Application.Mediatr` stays on nuget.org at 1.0.150 for existing
consumers — do not resurrect it here. `ValidationFilter<TRequest>` and `UnitOfWorkFilter`
in `src/Application/EndpointFilters/` cover what its behaviors did.

## Build and test

```bash
dotnet build --configuration Release      # whole solution
dotnet test --configuration Release       # whole suite — run it after each change
dotnet test tests/Domain.Tests --configuration Release   # single project
dotnet test --configuration Release --collect 'XPlat Code Coverage'
dotnet format --verify-no-changes --severity error   # what the linter workflow enforces
dotnet format --severity error                       # same check, but fixes in place
```

Test runner is **Microsoft.Testing.Platform** (`global.json` → `test.runner`) with
xUnit v3 (`xunit.v3.mtp-v2`). The suites are fast — run them all after every change.

`tests/Testing.Tests/Infrastructure/Persistence/` really does start containers
(`MariaDatabaseServerTests`, `PostgreDatabaseServerTests` via Testcontainers), so those
four tests need a working Docker endpoint. On a podman-socket setup they fail with
`Docker API responded with status code='InternalServerError' … operation not supported`
— an environment problem, not a regression. Everything else is unit-level.

`./build.sh` is the local packaging path only — see
`docs/contributing/tasks/test-package-locally.md`.

### The build is strict

The root `Directory.Build.props` sets `TreatWarningsAsErrors`,
`CodeAnalysisTreatWarningsAsErrors`, `AnalysisMode=All`, `AnalysisLevel=latest`,
`EnableNETAnalyzers`, `EnforceCodeStyleInBuild` and `Nullable=enable`. A warning
**is** a build failure, so "all warnings resolved" in the git workflow is satisfied
by a clean `dotnet build` — never silence one by widening the repo-wide `NoWarn`
(currently `CA1848` and `CS1591`, plus `S3925` in tests). Suppress a rule only at
the narrowest scope with a justification.

`GenerateDocumentationFile` is on for every project. CS1591 is suppressed, so a
missing XML comment will not fail the build — write one anyway on public members;
the published docs are derived from them.

`Directory.Build.targets` carries two things to respect:
- a `BeforeTargets="CoreCompile"` target that strips the `ReactiveUI.Primitives.R3Bridge`
  source generator (transitive via Refit 12) because its generated internal type
  collides through `InternalsVisibleTo` and fails with CS0436
- `DependentUpon` conventions nesting `*.Handler.cs`, `*.Request.cs`, `*.Response.cs`,
  `*.Validator.cs`, `*.When.cs`, `*.Then.cs` under their base file — follow that naming

Every source project exposes internals to `<AssemblyName>.Tests` and to
`DynamicProxyGenAssembly2` (NSubstitute), and every project gets a
`FrameworkReference` on `Microsoft.AspNetCore.App`.

## Package management

Versions are centralized — `ManagePackageVersionsCentrally=true`. Add a
`<PackageReference Include="X" />` with **no** `Version` attribute to the csproj,
and the `<PackageVersion>` to `Directory.Packages.props`. Per the engineering
principles, do not add a new dependency without approval.

`FluentAssertions` is pinned to 7.x deliberately (8.x changed its license). Do not
bump it to 8 or beyond. Renovate opens the routine dependency PRs; `NuGetAuditMode`
is `direct`.

## Test conventions

Match the existing suites:

- Class carries `[UnitTest]` (from `DrifterApps.Seeds.Testing.Attributes`, a
  `CategoryAttribute` trait).
- `GlobalUsings.cs` in each test project already covers
  `DrifterApps.Seeds.Testing.Attributes`, `FluentAssertions` and `Xunit` — do not
  re-import them per file.
- Random data comes from `private readonly Faker _faker = new();`, never hard-coded
  literals, unless the literal is the thing under test.
- Test names describe behavior: `GivenNew_WhenInvoked_ThenReturnNewId`.
- Bodies use explicit lowercase `// arrange` / `// act` / `// assert` sections.
- Parameterized cases use `public static TheoryData<…>` properties with `[Theory]`.
- Assert with FluentAssertions, plus the assertion helpers in
  `src/Testing/FluentAssertions/` where they fit.
- Test files mirror the source file they cover; test projects are `OutputType=Exe`
  with `RootNamespace`/`AssemblyName` set to `DrifterApps.Seeds.<Project>.Tests`.

CI reports coverage with thresholds `60 80`. New public behavior needs tests;
follow the TDD cycle in the engineering principles (failing test first).

## Documentation is part of the API

`src/Directory.Build.props` packs `docs/**` and `ARCHITECTURE.md` into **every**
package, and each project's own `src/<Project>/README.md` is its NuGet readme.
When the public surface changes, update in the same change:

- `src/<Project>/README.md` — that package's usage summary
- `docs/API.md` — types, signatures, extension points
- `docs/EXAMPLES.md` — usage patterns
- `docs/AI-GUIDELINES.md` — rules for assistants consuming the packages
- `ARCHITECTURE.md` — only when a design decision or the package map changes
- `examples/` — when the feature is best shown end-to-end
- the consumer rules files — `.junie/guidelines.md`, `.github/copilot-instructions.md`,
  `.cursor/rules/drifterapps-seeds.mdc`, `.windsurf/rules/drifterapps-seeds.md` (they
  carry the same content at four verbosity tiers; keep them in agreement)
- `llms.txt` — only when the package list or the doc map changes
- XML doc comments on the members themselves

`docs/contributing/**` is excluded from the pack — process docs never ship.
`docs/local-nuget-testing-guide.md` is the consumer-facing walkthrough of `build.sh`.

Treat a doc-only correction as its own `docs:` commit, separate from behavior.

## Design constraints

See `ARCHITECTURE.md` before changing any of these:

- **Result pattern over exceptions** — `DrifterApps.Seeds.FluentResult` carries
  expected failures; programmer errors stay exceptions
  (`ArgumentNullException.ThrowIfNull`).
- **Scenario testing** — `DrifterApps.Seeds.FluentScenario` backs `src/Testing`
  (`StepDefinitions/`, `Extensions/RefitExtensions.cs`).
- The graph above is load-bearing: `Domain` takes no dependency on another seed
  package, and nothing lower reaches up.
- Public surface is a NuGet contract. A breaking change needs a
  `+semver: breaking` commit and a note in the release changelog.

## Usage reference

Do not restate the libraries' usage rules here — they live in, and are kept current
by, the published docs:

- **`docs/API.md`** — every type and signature, per package
- **`docs/EXAMPLES.md`** — end-to-end usage patterns
- **`docs/AI-GUIDELINES.md`** — when to apply each building block, and the anti-patterns
- **`ARCHITECTURE.md`** — the package map and why the design is what it is
