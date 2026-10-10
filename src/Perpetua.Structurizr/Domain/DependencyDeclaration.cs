namespace Perpetua.Structurizr.Domain;

/// <summary>A container dependency declared in code.</summary>
public sealed record DependencyDeclaration(ContainerName Container, string? Context, string? Description);
