using Perpetua.Structurizr.Application;
using Perpetua.Structurizr.Domain;

namespace Perpetua.Structurizr.Tests;

internal sealed class InMemoryContainerSource(params ContainerAttribute[] containers) : IContainerSource
{
    public IEnumerable<ContainerAttribute> Containers() => containers;
}

internal sealed class InMemoryContainerDslOutput : IContainerDslOutput
{
    public List<ContextDslFragment> Written { get; } = [];

    public void Write(ContextDslFragment context) => Written.Add(context);
}
