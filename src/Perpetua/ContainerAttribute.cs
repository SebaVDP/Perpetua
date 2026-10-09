namespace Perpetua;

/// <summary>A C4 container is an independently runnable application or data store within a software system.</summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ContainerAttribute(string context, string name) : Attribute
{
    /// <summary>The name of the software-system context this container belongs to.</summary>
    public string Context { get; } = context;

    public string Name { get; } = name;
}
