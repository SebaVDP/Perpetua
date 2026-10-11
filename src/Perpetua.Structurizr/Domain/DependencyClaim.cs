namespace Perpetua.Structurizr.Domain;

/// <summary>
/// What the code claims about a dependency of the container, exactly as written and not yet checked,
/// together with the class that claims it so a problem can be found again.
/// </summary>
public sealed record DependencyClaim(string Identifier, string? Context, string? Description, DeclaringClass DeclaringClass);
