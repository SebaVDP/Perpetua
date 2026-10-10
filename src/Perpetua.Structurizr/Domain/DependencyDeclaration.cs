namespace Perpetua.Structurizr.Domain;

/// <summary>A container dependency declared in code.</summary>
public sealed record DependencyDeclaration(string Identifier, string? Context, string? Description);
