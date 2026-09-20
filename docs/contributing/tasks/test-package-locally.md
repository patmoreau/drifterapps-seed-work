---
name: test-package-locally
description: Pack the seed packages locally with build.sh and consume them from another solution before release
---

# Test a package locally

> **Audience:** contributors *working on* this repository. Not packaged — see `AGENTS.md` at the repository root.

Use this when a change needs to be proven from a *consuming* solution
(holefeeder, an example app) before it is released. The consumer-facing walkthrough
is `docs/local-nuget-testing-guide.md`; this is the procedure to run.

## 1. Pack

```bash
./build.sh
```

`build.sh` increments the counter in `version.txt`, clears `./nuget-packages/`,
builds and tests in **Debug**, packs `1.0.<counter>-alpha`, and pushes to the local
feed at `~/.nuget/local-packages/`. It aborts nothing on test failure, so read the
output: a red test run still produces packages, and those must not be consumed.

The version it just produced:

```bash
echo "1.0.$(cat version.txt)-alpha"
```

`version.txt` is a local counter only — gitignored, and unrelated to the released
version, which GitVersion owns.

## 2. Consume from the other solution

The consuming project needs the local feed in its `nuget.config`:

```xml
<packageSources>
  <clear />
  <add key="local" value="/Users/patrickmoreau/.nuget/local-packages/" />
  <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
</packageSources>
```

Then pin the alpha version explicitly (central package management: the version
goes in that repo's `Directory.Packages.props`):

```bash
dotnet nuget locals http-cache --clear
dotnet restore
```

Pre-release versions are not picked up by a floating range, so state the exact
`1.0.<counter>-alpha`.

## 3. Verify, then clean up

Build and run the consumer's tests against the alpha package and report what it
proved. Afterwards:

- Restore the consuming project to the released package version — never commit a
  `1.0.x-alpha` reference in another repo.
- `./nuget-packages/` is generated output — never commit its contents.

## Never

- Publish a local alpha to nuget.org — releases come from
  `docs/contributing/tasks/create-github-release.md` only.
- Consume packages from a `build.sh` run whose tests failed.
