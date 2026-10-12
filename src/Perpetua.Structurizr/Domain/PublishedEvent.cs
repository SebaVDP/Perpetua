namespace Perpetua.Structurizr.Domain;

/// <summary>
/// An event that a deployment unit publishes and that other deployment units can depend on.
/// An event is documented as a container named after its publisher and the event, so the same event name
/// can be published by different deployment units; the convention lives here so consumers never need to know it.
/// </summary>
public sealed record PublishedEvent(ContainerName Name)
{
    public static string ContainerNameFor(string publisher, string eventName) => $"{publisher}_{eventName}";
}
