namespace Perpetua.Structurizr.Domain;

/// <summary>
/// The outcome of declaring the container found in a run.
/// It is exactly one of the nested cases; the private constructor keeps the set closed.
/// </summary>
public abstract record ContainerDeclarationResult
{
    private ContainerDeclarationResult()
    {
    }

    /// <summary>The deployment unit declares exactly one container, and this is the declaration that documents it.</summary>
    public sealed record DeclarationGenerated(ContextDeclaration Declaration) : ContainerDeclarationResult;

    /// <summary>The deployment unit declares no container, so there is nothing to document. This is not a failure.</summary>
    public sealed record NoContainersFound : ContainerDeclarationResult;

    /// <summary>
    /// A deployment unit declared more than one container. Each deployment unit is documented by its own run,
    /// so the run fails and generates nothing.
    /// </summary>
    public sealed record OnlyOneContainerAllowed : ContainerDeclarationResult;

    /// <summary>
    /// The container name cannot be used as a Structurizr identifier or as a folder name,
    /// so the run fails and generates nothing.
    /// </summary>
    public sealed record InvalidContainerName : ContainerDeclarationResult;

    /// <summary>
    /// The context cannot be used as a Structurizr identifier or as a folder name,
    /// so the run fails and generates nothing.
    /// </summary>
    public sealed record InvalidContextName : ContainerDeclarationResult;

    /// <summary>
    /// A dependency identifier cannot be used as a Structurizr identifier,
    /// so the run fails and generates nothing.
    /// </summary>
    public sealed record InvalidDependencyIdentifier : ContainerDeclarationResult;

    /// <summary>
    /// A dependency context cannot be used as a Structurizr identifier,
    /// so the run fails and generates nothing.
    /// </summary>
    public sealed record InvalidDependencyContext : ContainerDeclarationResult;

    /// <summary>
    /// The same dependency (target, context and description) is declared more than once on the container.
    /// Structurizr rejects a duplicate relationship, so the run fails and generates nothing; the user resolves it.
    /// </summary>
    public sealed record DuplicateDependency : ContainerDeclarationResult;
}
