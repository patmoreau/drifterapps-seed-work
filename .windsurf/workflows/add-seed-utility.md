---
description: Add or change a public utility in one of the DrifterApps.Seeds packages, with the right project, tests and docs
---

<!-- Generated from docs/contributing/tasks/add-seed-utility.md by scripts/sync-agent-files.sh. Do not edit. -->

# Add a seed utility

> **Audience:** contributors *working on* this repository. Not packaged — see `AGENTS.md` at the repository root.

Everything public here is a NuGet surface that downstream apps compile against.
The work is: pick the right package, TDD the behavior, then update the docs that
ship with the package.

## 1. Pick the package

The dependency graph is one-directional and must stay that way:

```
Domain  ←  Application  ←  Application.Mediatr
               ↑
         Infrastructure
Testing  →  Domain, Infrastructure
```

| Put it in | When |
|---|---|
| `src/Domain/` | DDD contracts and primitives — aggregate roots, strongly-typed IDs, repositories, unit of work. No ASP.NET, no infrastructure. |
| `src/Application/` | Application-layer plumbing on ASP.NET Core — authorization, endpoint filters, converters, query params, DI extensions |
| `src/Application.Mediatr/` | MediatR pipeline behaviors and FluentValidation mapping only |
| `src/Infrastructure/` | Outbound technology adapters — Hangfire scheduling, Refit HTTP support |
| `src/Testing/` | Test infrastructure consumers use in *their* test suites — `FakerBuilder`, drivers, assertions, trait attributes |

If the new type would force a package to reference something further down the
list, it belongs in the lower package instead. Never add a reference that creates
a cycle or points a lower package at a higher one.

## 2. Write the failing test first

Test projects mirror the source projects: `tests/Domain.Tests`,
`tests/Application.Tests`, `tests/Infrastructure.Tests`, `tests/Testing.Tests`.

`src/Application.Mediatr` has **no test project and is not in the solution**, so
CI never builds it and the release never publishes it (stuck at 1.0.150). If the
change lands there, say so explicitly and build it directly
(`dotnet build src/Application.Mediatr`); putting it back in the solution, or
adding its test project, is separate work that needs approval.

Follow the existing suite's conventions (see `AGENTS.md` → "Test conventions"):
`[UnitTest]`, a `private readonly Faker _faker = new();`,
`GivenX_WhenY_ThenZ` names, lowercase `// arrange` / `// act` / `// assert`
sections, `TheoryData<…>` for parameterized cases.

Then the TDD cycle from the engineering principles: one failing test, minimum
code to pass, refactor, `dotnet test` after every step.

## 3. Mind the strict build

`TreatWarningsAsErrors`, `CodeAnalysisTreatWarningsAsErrors` and `AnalysisMode=All`
are on, so an analyzer complaint is a build failure. Fix it rather than widening
`NoWarn` in `Directory.Build.props`; a genuine false positive gets a narrowly
scoped `[SuppressMessage(…)]` with a justification.

Public members want XML doc comments — `GenerateDocumentationFile` is on, and the
comments feed the docs below — even though CS1591 is suppressed repo-wide.

A new dependency needs approval, and its version goes in
`Directory.Packages.props` (never a `Version` attribute in a csproj).

## 4. Update the docs in the same change

`src/Directory.Build.props` packs `docs/**` (minus `docs/contributing/`) and
`ARCHITECTURE.md` into **every** package, and each project's own `README.md` is
its NuGet readme. When the public surface changes, update:

- `src/<Package>/README.md` — the package's own usage summary
- `docs/API.md` — signatures, overloads, extension points
- `docs/EXAMPLES.md` — a usage pattern for anything non-obvious
- `docs/AI-GUIDELINES.md` — the rule an assistant needs to use it correctly
- the consumer rules files — `.junie/guidelines.md`, `.github/copilot-instructions.md`,
  `.cursor/rules/drifterapps-seeds.mdc`, `.windsurf/rules/drifterapps-seeds.md` — when
  the change touches anything they restate
- `llms.txt` — only when the package list or the doc map changes
- `ARCHITECTURE.md` — only when a design decision or the package map changes
- `examples/` — when the feature is best shown end-to-end (these are illustrative
  sources, not a compiled project)

## 5. Finish

```bash
dotnet build
dotnet test
dotnet format --severity error
```

Commit per `docs/contributing/git-instructions.md`: structural changes first and
separate, `feat(<package>): …` for the behavior, a `+semver:` trailer in the body
when consumers will see the change, and a `docs:` commit of its own for doc-only
corrections.

## Never

- Introduce a dependency between seed packages that contradicts the graph above.
- Add a public member with no test.
- Change a public signature without updating `docs/API.md` and the package README.
