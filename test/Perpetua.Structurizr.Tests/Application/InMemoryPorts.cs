using Perpetua.Structurizr.Application;
using Perpetua.Structurizr.Domain;

namespace Perpetua.Structurizr.Tests;

internal sealed class InMemoryContainerSource : IDeclarationSource
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

    public IEnumerable<Declared<ContainerAttribute>> Containers()
        => containers.Select(container => new Declared<ContainerAttribute>(container, new DeclaringClass("Sample.Fixture")));

    public IEnumerable<Declared<DependsOnAttribute>> Dependencies()
        => dependencies.Select(dependency => new Declared<DependsOnAttribute>(dependency, new DeclaringClass("Sample.Fixture")));
}

internal sealed class InMemoryContextDeclarationDslOutput : IContextDeclarationDslOutput
{
    public List<ContextDeclaration> Written { get; } = [];

    public void Write(ContextDeclaration context) => Written.Add(context);
}
