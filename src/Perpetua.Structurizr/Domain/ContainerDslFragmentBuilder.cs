using Perpetua;

namespace Perpetua.Structurizr.Domain;

using static ContainerDslFragmentResult;

/// <summary>
/// Decides what a run produces from the containers its deployment unit declares:
/// none means nothing to document, exactly one is documented, and more than one is refused
/// because each deployment unit is documented by its own run.
/// </summary>
public sealed class ContainerDslFragmentBuilder
{
    public ContainerDslFragmentResult Build(IEnumerable<ContainerAttribute> containers)
        => containers.ToList() switch
        {
            [] => new NoContainersFound(),
            [var container] => new FragmentGenerated(new ContextDslFragment(
                container.Context,
                new ContainerDslFragment(container.Name))),
            _ => new OnlyOneContainerAllowed()
        };
}
