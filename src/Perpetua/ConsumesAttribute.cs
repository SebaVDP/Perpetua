namespace Perpetua;

/// <summary>Declares that the class consumes an event that a publisher container publishes.</summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class ConsumesAttribute(string publisher, string eventName, string? context = null, string? description = null) : Attribute
{
    /// <summary>The name of the container that publishes the event.</summary>
    public string Publisher { get; } = publisher;

    public string EventName { get; } = eventName;

    /// <summary>The software-system context of the publisher; defaults to the context of the consumer.</summary>
    public string? Context { get; } = context;

    public string? Description { get; } = description;
}
