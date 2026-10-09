namespace Perpetua.Structurizr.Tests.Fixtures;

/// <summary>A declared container used to exercise assembly scanning in tests.</summary>
[Container("SampleSystem", "SampleContainer")]
public sealed class SampleContainer;

[Container("SampleSystem", "SampleWorker")]
public sealed class SampleWorker;

[Container("OtherSystem", "OtherContainer")]
public sealed class OtherContainer;

[Container("OtherSystem", "OtherWorker")]
public sealed class OtherWorker;
