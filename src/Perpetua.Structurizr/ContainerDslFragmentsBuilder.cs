using Perpetua;

namespace Perpetua.Structurizr;

public sealed class ContainerDslFragmentsBuilder
{
    public ContainerDslFragmentsResult Build(IEnumerable<ContainerAttribute> containers)
    {
        if (containers.Count() > 1)
        {
            return new OnlyOneContainerAllowed();
        }

        var contextFragments = containers
            .Select(container => new ContextDslFragment(
                container.Context,
                [new ContainerDslFragment(container.Name)]))
            .ToList();

        return new ContainerDslFragments(contextFragments);
    }
}
