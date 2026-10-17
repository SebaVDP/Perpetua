namespace Perpetua;

/// <summary>Declares that the class consumes an event that a publisher container publishes.</summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class ConsumesAttribute(string eventName, string publisherContainer, string? publisherContext = null, string? description = null) : Attribute
{
    /// <summary>The name of the event published by the container.</summary>
    public string EventName { get; } = eventName;

    /// <summary>The name of the container that publishes the event.</summary>
    public string Publisher { get; } = publisherContainer;

    /// <summary>The software-system context of the publisher; defaults to the context of the consumer.</summary>
    public string? Context { get; } = publisherContext;

    /// <summary>Why this class consumes the event.</summary>
    public string? Description { get; } = description;
}
