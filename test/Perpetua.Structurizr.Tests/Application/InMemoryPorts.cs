using Perpetua.Structurizr.Application;
using Perpetua.Structurizr.Domain;

namespace Perpetua.Structurizr.Tests;

internal sealed class InMemoryContainerSource : IContainerSource
{
    private readonly ContainerAttribute[] containers;
    private readonly DependsOnAttribute[] dependencies;

    public InMemoryContainerSource(params ContainerAttribute[] containers)
        : this(containers, [])
    {
    }

    public InMemoryContainerSource(ContainerAttribute[] containers, params DependsOnAttribute[] dependencies)
    {
        this.containers = containers;
        this.dependencies = dependencies;
    }

    public IEnumerable<ContainerAttribute> Containers() => containers;

    public IEnumerable<DependsOnAttribute> Dependencies() => dependencies;
}

internal sealed class InMemoryContainerDslOutput : IContainerDslOutput
{
    public List<ContextDeclaration> Written { get; } = [];

    public void Write(ContextDeclaration context) => Written.Add(context);
}
