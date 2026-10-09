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
    A["Billing service<br/>[Container('Billing', 'Invoicing')]"] -->|"perpetua-structurizr<br/>(one run per deployment unit)"| B["docs/Billing/containers/Invoicing.dsl"]
    C["Accounts service<br/>[Container('Accounts', 'Website')]"] -->|"perpetua-structurizr<br/>(one run per deployment unit)"| D["docs/Accounts/containers/Website.dsl"]
    B --> W["workspace.dsl<br/>(hand-written)"]
    D --> W
    W -->|"!include &lt;context&gt;/containers"| S["Structurizr"]
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

Run the tool once per deployment unit, passing the directory with its assemblies and, optionally, the output directory (both default to the current directory):

```bash
perpetua-structurizr ./billing/bin/Release/net10.0 ./docs
perpetua-structurizr ./accounts/bin/Release/net10.0 ./docs
```

The output directory now contains one folder per context:

```
docs/
├── Billing/
│   └── containers/
│       └── Invoicing.dsl      Invoicing = container "Invoicing"
└── Accounts/
    └── containers/
        └── Website.dsl        Website = container "Website"
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
    }
}
```

Separate runs can therefore declare containers with the same name in different contexts.

### Rules

- A deployment unit declares **one** container. More than one fails the run and generates nothing; none generates nothing and succeeds.
- The context and the container name become a folder and a Structurizr identifier, so they may only contain letters, digits, `_` and `-`, and may not start with `-`. Anything else fails the run.
- Files from an earlier run with the same context and name are overwritten.

## License

[MIT](LICENSE)
