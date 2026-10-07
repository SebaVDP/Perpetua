# Cairn

[![CI](https://github.com/SebaVDP/Cairn/actions/workflows/ci.yml/badge.svg)](https://github.com/SebaVDP/Cairn/actions/workflows/ci.yml)
[![NuGet (Cairn)](https://img.shields.io/nuget/v/Cairn?label=Cairn)](https://www.nuget.org/packages/Cairn)
[![NuGet (Cairn.Structurizr)](https://img.shields.io/nuget/v/Cairn.Structurizr?label=Cairn.Structurizr)](https://www.nuget.org/packages/Cairn.Structurizr)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

Cairn - embed meaningful markers in your code; tools follow them to generate documentation (C4/Structurizr, flows, onboarding).

## Install

```bash
dotnet add package Cairn
```

## Usage

```csharp
using Cairn;

var dsl = new Generator().Generate();
// dsl is a valid Structurizr workspace.dsl
```

## License

[MIT](LICENSE)
