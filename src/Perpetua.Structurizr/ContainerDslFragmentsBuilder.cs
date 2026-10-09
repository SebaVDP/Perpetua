using Perpetua;

namespace Perpetua.Structurizr;

public sealed class ContainerDslFragmentsBuilder
{
    public ContainerDslFragmentsResult Build(IEnumerable<ContainerAttribute> containers)
    {
        var contextFragments = containers
            .GroupBy(container => container.Context)
            .OrderBy(context => context.Key, StringComparer.Ordinal)
            .Select(context => new ContextDslFragment(
                context.Key,
                context.OrderBy(container => container.Name, StringComparer.Ordinal)
                    .Select(container => new ContainerDslFragment(container.Name))
                    .ToList()))
            .ToList();

        return new ContainerDslFragments(contextFragments);
    }
}
