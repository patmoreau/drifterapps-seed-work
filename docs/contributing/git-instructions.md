# Git Instructions

> **Audience:** contributors *working on* this repository. Not packaged — see [AGENTS.md](../../AGENTS.md).

## Commit Branching Strategy

1. Commit directly to main unless branching is specified.

## Commit Discipline

Only commit when:

1. ALL tests are passing
2. ALL compiler/linter warnings resolved
3. Single logical unit of work
4. I have performed a code review myself

Never mix structural and behavioral changes in the same commit.
Always make structural changes first when both are needed.

## Commit Message Rules

- Use Conventional Commits (`type(scope): subject`)
- Types: feat, fix, refactor, test, docs, chore, build, ci
- Scopes: the package touched — `domain`, `application`, `infrastructure`, `testing`
  — or `docs`, `ci`, `deps` for cross-cutting work
- Structural changes use `refactor` or `chore`; behavioral changes use `feat` or `fix`
- Subject line max 50 characters, capitalized, no trailing period
- Separate subject from body with blank line
- Wrap body at 72 characters
- Imperative mood ("Add unit tests" not "Added unit tests")
- Add co-authored tag with agent name and model used similar to this
  `Co-Authored-By: Claude Opus 5 <noreply@anthropic.com>`

## Version bumps

GitVersion runs in ContinuousDelivery mode with `commit-message-incrementing: Enabled`,
so **the released version only moves when a commit message carries a `+semver:` trailer**:

- `+semver: breaking` / `+semver: major`
- `+semver: feature` / `+semver: minor`
- `+semver: fix` / `+semver: patch`

A behavioral change that consumers will see needs one. Put it in the commit body.

## Push to origin
- Watch the ci workflows for success:
  - ci-cd
  - codeql-analysis
  - linter

## Never

- Commit secrets or confidential information
- Push to GitHub by yourself
