namespace Perpetua;

/// <summary>Declares a container that this class depends on.</summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class DependsOnAttribute(string name, string? context = null, string? description = null) : Attribute
{
    /// <summary>The name of the container this class depends on.</summary>
    public string Name { get; } = name;

    /// <summary>The software-system context of the dependency; defaults to the context of the declaring container.</summary>
    public string? Context { get; } = context;

    /// <summary>Why this dependency exists.</summary>
    public string? Description { get; } = description;
}
