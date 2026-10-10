namespace Perpetua.Structurizr.Tests.Fixtures;

/// <summary>A declared container used to exercise assembly scanning in tests.</summary>
[Container("SampleSystem", "SampleContainer")]
[DependsOn("Api")]
[DependsOn("Ledger", "Accounts")]
[DependsOn("Audit", "Compliance", "Records changes")]
public sealed class SampleContainer;
