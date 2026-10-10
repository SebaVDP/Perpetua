namespace Perpetua.Structurizr.Tests.Fixtures;

/// <summary>A declared container used to exercise assembly scanning in tests.</summary>
[Container("SampleSystem", "SampleContainer")]
[DependsOn("Api")]
[DependsOn("Ledger", "Accounts")]
public sealed class SampleContainer;
