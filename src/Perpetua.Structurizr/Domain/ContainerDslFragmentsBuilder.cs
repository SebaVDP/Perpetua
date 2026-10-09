using Perpetua;

namespace Perpetua.Structurizr.Domain;

public sealed class ContainerDslFragmentsBuilder
{
    public ContainerDslFragmentsResult Build(IEnumerable<ContainerAttribute> containers)
        => containers.ToList() switch
        {
            [] => new NoContainersFound(),
            [var container] => new ContextDslFragment(
                container.Context,
                new ContainerDslFragment(container.Name)),
            _ => new OnlyOneContainerAllowed()
        };
}
