namespace Perpetua;

/// <summary>Declares an event that the class publishes, so consumers can depend on it.</summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class EventAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}
