namespace Perpetua.Structurizr.Domain;

/// <summary>
/// The outcome of building the container DSL fragment from the containers found in a run.
/// It is exactly one of the nested cases; the private constructor keeps the set closed.
/// </summary>
public abstract record ContainerDslFragmentResult
{
    private ContainerDslFragmentResult()
    {
    }

    /// <summary>The deployment unit declares exactly one container, and this is the fragment that documents it.</summary>
    public sealed record FragmentGenerated(ContextDslFragment Fragment) : ContainerDslFragmentResult;

    /// <summary>The deployment unit declares no container, so there is nothing to document. This is not a failure.</summary>
    public sealed record NoContainersFound : ContainerDslFragmentResult;

    /// <summary>
    /// A deployment unit declared more than one container. Each deployment unit is documented by its own run,
    /// so the run fails and generates nothing.
    /// </summary>
    public sealed record OnlyOneContainerAllowed : ContainerDslFragmentResult;
}
