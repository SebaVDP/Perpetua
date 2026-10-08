namespace Perpetua;

/// <summary>Declares a container and the software system it belongs to.</summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ContainerAttribute(string context, string name) : Attribute
{
    /// <summary>Gets the system context that owns the container.</summary>
    public string Context { get; } = context;

    /// <summary>Gets the container name.</summary>
    public string Name { get; } = name;
}
