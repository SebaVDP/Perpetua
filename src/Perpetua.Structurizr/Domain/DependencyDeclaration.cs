namespace Perpetua.Structurizr.Domain;

/// <summary>A container dependency declared in code.</summary>
public sealed record DependencyDeclaration(ContainerName Container, ContextName? Context, string? Description);
