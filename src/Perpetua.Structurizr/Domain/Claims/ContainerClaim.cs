namespace Perpetua.Structurizr.Domain;

/// <summary>
/// What the code claims about a container, exactly as written and not yet checked,
/// together with the class that claims it so a problem can be found again.
/// </summary>
public sealed record ContainerClaim(string Context, string Name, DeclaringClass DeclaringClass);
