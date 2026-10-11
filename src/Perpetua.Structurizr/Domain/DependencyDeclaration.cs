namespace Perpetua.Structurizr.Domain;

/// <summary>A container dependency declared in code. The context is always known: it defaults to the context of the declaring container.</summary>
public sealed record DependencyDeclaration(ContainerName Container, ContextName Context, string? Description);
