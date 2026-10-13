namespace Perpetua.Structurizr.Domain;

/// <summary>
/// A container identified together with its software-system context, which is how Structurizr
/// refers to it in a relationship, because the same container name can exist in several contexts.
/// </summary>
public sealed record QualifiedContainerName(ContextName Context, ContainerName Container)
{
    public override string ToString() => $"{Context}.{Container}";
}
