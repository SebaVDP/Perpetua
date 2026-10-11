# Perpetua

[![CI](https://github.com/SebaVDP/Perpetua/actions/workflows/ci.yml/badge.svg)](https://github.com/SebaVDP/Perpetua/actions/workflows/ci.yml)
[![NuGet (Perpetua)](https://img.shields.io/nuget/v/Perpetua?label=Perpetua)](https://www.nuget.org/packages/Perpetua)
[![NuGet (Perpetua.Structurizr)](https://img.shields.io/nuget/v/Perpetua.Structurizr?label=Perpetua.Structurizr)](https://www.nuget.org/packages/Perpetua.Structurizr)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

Perpetua is a living-documentation platform: embed meaningful markers in your code and generate always-current documentation (C4/Structurizr, flows, onboarding).

## Install

```bash
dotnet add package Perpetua
dotnet tool install --global Perpetua.Structurizr
```

## Usage

### How it fits together

```mermaid
flowchart LR
    A["Billing service<br/>[Container('Billing', 'Invoicing')]"] -->|"perpetua-structurizr<br/>(one run per deployment unit)"| B["docs/Billing/containers/Invoicing.dsl<br/>docs/Billing/relationships/Invoicing.dsl"]
    C["Accounts service<br/>[Container('Accounts', 'Website')]"] -->|"perpetua-structurizr<br/>(one run per deployment unit)"| D["docs/Accounts/containers/Website.dsl<br/>docs/Accounts/relationships/Website.dsl"]
    B --> W["workspace.dsl<br/>(hand-written)"]
    D --> W
    W -->|"!include &lt;context&gt;/containers<br/>then !include &lt;context&gt;/relationships"| S["Structurizr"]
```

Each deployment unit is documented by its own run. The hand-written workspace brings the generated folders together.

### Example

Two deployment units, each marking its entry point with the container it represents:

```csharp
// Billing service
[Container("Billing", "Invoicing")]
public class InvoicingService { }

// Accounts service
[Container("Accounts", "Website")]
public class WebsiteHost { }
```

A container declares the other containers it depends on with `[DependsOn]`, on any class of the same deployment unit:

```csharp
// Billing service
[DependsOn("Website", "Accounts", "Looks up customers")]
public class CustomerLookup { }

[DependsOn("Ledger")]
public class PostingJob { }
```

`[DependsOn(identifier, context = null, description = null)]`:

- The identifier of the target container is required.
- The context is optional and defaults to the context of the container of the deployment unit (`Billing` here, so `Ledger` is `Billing.Ledger`).
- The description is optional and becomes the description of the relationship.

Run the tool once per deployment unit, passing the directory with its assemblies and, optionally, the output directory (both default to the current directory):

```bash
perpetua-structurizr ./billing/bin/Release/net10.0 ./docs
perpetua-structurizr ./accounts/bin/Release/net10.0 ./docs
```

The output directory now contains one folder per context:

```
docs/
├── Billing/
│   ├── containers/
│   │   └── Invoicing.dsl      Invoicing = container "Invoicing"
│   └── relationships/
│       └── Invoicing.dsl      Billing.Invoicing -> Accounts.Website "Looks up customers"
│                              Billing.Invoicing -> Billing.Ledger
└── Accounts/
    ├── containers/
    │   └── Website.dsl        Website = container "Website"
    └── relationships/
        └── Website.dsl        (empty: no dependencies declared)
```

Include the folders in a hand-written workspace. It uses `!identifiers hierarchical`, and the context must equal the identifier of the `softwareSystem` that includes its folder:

```
workspace {
    !identifiers hierarchical
    model {
        Billing = softwareSystem "Billing" {
            !include docs/Billing/containers
        }
        Accounts = softwareSystem "Accounts" {
            !include docs/Accounts/containers
        }
        !include docs/Billing/relationships
        !include docs/Accounts/relationships
    }
}
```

Include every `containers` folder first, then every `relationships` folder. A relationship can point to a container of another context, so all containers must be declared before any relationship is read.

Separate runs can therefore declare containers with the same name in different contexts.

### Rules

- A deployment unit declares **one** container. More than one fails the run and generates nothing; none generates nothing and succeeds.
- The context and the container name become a folder and a Structurizr identifier, so they may only contain letters, digits, `_` and `-`, and may not start with `-`. Anything else fails the run. The same goes for the identifier and context of a `[DependsOn]`.
- A relationship is identified by its target, its context and its description. Declaring the same one twice fails the run, so remove the duplicate. The same target with a different description is allowed, and so is the same container in two contexts.
- Files from an earlier run with the same context and name are overwritten.

### Failures

A failed run generates nothing and exits with code `1` (success is `0`). It reports every problem it found, one per line, as a code and a message that names the offending value and the class that declares it, for example:

```
PSTRUCT.0004: Dependency identifier 'Payment.Gateway' on class 'Billing.PaymentClient' is not a valid identifier.
```

| Code | Problem |
|------|---------|
| `PSTRUCT.0001` | A deployment unit declares more than one container. |
| `PSTRUCT.0002` | The container name is not a valid identifier. |
| `PSTRUCT.0003` | The context is not a valid identifier. |
| `PSTRUCT.0004` | A dependency identifier is not a valid identifier. |
| `PSTRUCT.0005` | A dependency context is not a valid identifier. |
| `PSTRUCT.0006` | A dependency is declared twice with the same context and description. |

## License

[MIT](LICENSE)
