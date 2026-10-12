namespace Perpetua.Structurizr.Domain;

/// <summary>An event that a deployment unit publishes and that other deployment units can depend on.</summary>
public sealed record PublishedEvent(ContainerName Name);
