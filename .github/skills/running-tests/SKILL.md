---
name: running-tests
description: >
  How to run the Perpetua test suite and Stryker mutation testing. Triggers when
  running tests, checking results, or running mutation testing in this repo.
license: MIT
---

# Running tests and mutation testing

Perpetua uses **xUnit v3**, which runs on the **Microsoft Testing Platform (MTP)**, not VSTest.

## Tests

    dotnet run --project test\Perpetua.Tests\Perpetua.Tests.csproj

`dotnet test` is unreliable here (the MTP `dotnet test` harness reports
"Zero tests ran" on this machine), so run the test project directly.

## Mutation testing (Stryker)

From `test/Perpetua.Tests/` (run `dotnet tool restore` first if needed):

    dotnet stryker --project Perpetua.Structurizr.csproj --test-runner mtp

`--test-runner mtp` is required because Stryker's default VSTest runner
cannot launch an xUnit v3 (MTP) test host. Point `--project` at the source
project under test for the current slice.
