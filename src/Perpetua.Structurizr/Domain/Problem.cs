namespace Perpetua.Structurizr.Domain;

/// <summary>
/// Something in the declarations that makes the run fail and generate nothing; the user resolves it.
/// It is exactly one of the nested cases; the private constructor keeps the set closed.
/// </summary>
public abstract record Problem
{
    private Problem()
    {
    }

    /// <summary>
    /// A deployment unit declared more than one container. Each deployment unit is documented by its own run.
    /// </summary>
    public sealed record OnlyOneContainerAllowed : Problem;

    /// <summary>The container name cannot be used as a Structurizr identifier or as a folder name.</summary>
    public sealed record InvalidContainerName : Problem;

    /// <summary>The context cannot be used as a Structurizr identifier or as a folder name.</summary>
    public sealed record InvalidContextName : Problem;

    /// <summary>A dependency identifier cannot be used as a Structurizr identifier.</summary>
    public sealed record InvalidDependencyIdentifier(string Value, string DeclaringClass) : Problem;

    /// <summary>A dependency context cannot be used as a Structurizr identifier.</summary>
    public sealed record InvalidDependencyContext : Problem;

    /// <summary>
    /// The same dependency (target, context and description) is declared more than once on the container.
    /// Structurizr rejects a duplicate relationship.
    /// </summary>
    public sealed record DuplicateDependency : Problem;
}
