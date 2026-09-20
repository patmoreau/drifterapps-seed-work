@AGENTS.md

## Claude Code

The instructions above apply in full. A few Claude-specific notes:

- Verify against the source, not from memory of this file — the code under `src/` and
  its XML doc comments are the truth. Documentation in `docs/` and `ARCHITECTURE.md`
  can drift; when it disagrees with the code, the code wins and the doc is a bug to fix.
- Prefer `dotnet build` / `dotnet test` from the repo root over per-project invocations;
  the suites are fast enough to run after every change, as the TDD cycle requires.
  Remember the root commands resolve `DrifterApps.Seeds.sln`, which omits
  `src/Application.Mediatr` — build that one explicitly when you touch it.
- Before proposing a new package, check `Directory.Packages.props` — it may already be
  there. Adding one needs approval either way.
- A change to a public type is a NuGet contract change: update that package's
  `src/<Project>/README.md` and `docs/API.md` in the same commit.
- Commits: conventional-commit format from `docs/contributing/git-instructions.md`,
  structural and behavioral changes never mixed, `+semver:` trailer when consumers
  see the change, and never push.
