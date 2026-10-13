namespace Perpetua.Structurizr.Domain;

/// <summary>
/// What the code claims about an event a class consumes, exactly as written and not yet checked,
/// together with the class that claims it so a problem can be found again.
/// </summary>
public sealed record ConsumptionClaim(string Publisher, string EventName, string? Context, string? Description, DeclaringClass DeclaringClass);
