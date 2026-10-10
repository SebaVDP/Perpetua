namespace Perpetua;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class DependsOnAttribute(string identifier, string? context = null, string? description = null) : Attribute
{
    public string Identifier { get; } = identifier;

    public string? Context { get; } = context;

    public string? Description { get; } = description;
}
