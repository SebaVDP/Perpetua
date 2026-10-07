# Development Workflow

This is Perpetua — a living-documentation platform that embeds meaningful markers in code so other tools can follow them to generate always-current documentation (C4/Structurizr, flows, onboarding) — built in C# (.NET 10) using **TDD** and **DDD**, with small, user-story-following git commits. Work proceeds story-by-story from the backlog tracked as **GitHub Issues**.

## TDD cycle (definition of done)

Every change follows this loop:

1. **RED** — write exactly one new failing test. Observe a clean assertion failure (not a compile error) before writing production code.
2. **GREEN** — write the minimum production code to pass.
3. **Refactor** — improve design while keeping the suite green. This is a deliberate, mandatory checkpoint, not an afterthought: **before running Stryker, always pause and explicitly ask both yourself and the user whether any refactoring should happen now** — in the production code *and* the test/fixture code (duplication, naming, misplaced responsibilities, redundant `using`s, scenario cohesion, etc.). Only proceed to Stryker once refactoring has been consciously considered and either done or declined.
4. **Run Stryker** — run mutation testing to check coverage before committing.
5. **Show the commit message** — present the proposed commit message for approval.
6. **Commit** — only after approval. Never commit without showing the message first.

_Exact test and mutation-testing commands live in the `running-tests` skill._

## Conventions

- **Small commits** that follow a single user story / acceptance criterion.
- **Preparatory refactor first (Kent Beck):** *"First make the change easy, then make the easy change."* When a new behaviour is hard to add, split the work into two commits: (1) a behaviour-preserving refactor that reshapes the design so the feature becomes trivial (all existing tests stay green), then (2) the feature itself as a small RED→GREEN slice. Never mix a structural refactor and a behavioural change in the same commit.
- **DDD error modeling:** user-supplied invalid input is a **domain error** (a `Result` failure, communicated gracefully); truly impossible states are invariant guards (throw).
- **Test placement — favor behavior over structure.** Write tests against the outermost boundary that expresses the behavior, so they assert *what the system does for a user*, not *how the code is arranged internally*. Concretely, exercise behavior through the use-case boundary (Application handlers/scenarios) whenever a user action drives it — this keeps tests resilient when responsibilities move between domain types during refactoring. Drop to a Domain unit test only for an invariant that is intrinsic to a single object in isolation and has no meaningful use-case expression. Prefer the higher-level test when both are possible; a test that has to reach into a specific aggregate to observe a behavior is a smell that it is coupled to structure.
- A **Probity preToolUse hook** (`.github/hooks/probity.json`) enforces strict TDD: one new test per write, and production code only after a clean RED.
- **Document meaning, not mechanics.** Code should explain how the software works through names, types, and structure. Documentation should capture meaning, intent, rationale, and domain or architectural knowledge that cannot reasonably be expressed by the code itself. Because Perpetua generates documentation from code, these descriptions remain part of the living source of truth.
