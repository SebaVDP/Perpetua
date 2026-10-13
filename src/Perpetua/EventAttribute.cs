namespace Perpetua;

/// <summary>Declares an event that the class publishes, so consumers can depend on it.</summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class EventAttribute(string name) : Attribute
{
    /// <summary>The name of the event published by the declaring class.</summary>
    public string Name { get; } = name;
}
