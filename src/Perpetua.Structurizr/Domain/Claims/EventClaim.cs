namespace Perpetua.Structurizr.Domain;

/// <summary>
/// What the code claims about an event a class publishes, exactly as written and not yet checked,
/// together with the class that claims it so a problem can be found again.
/// </summary>
public sealed record EventClaim(string Name, DeclaringClass DeclaringClass);
