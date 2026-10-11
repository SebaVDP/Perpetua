using Perpetua.Structurizr.Application;
using Perpetua.Structurizr.Domain;

namespace Perpetua.Structurizr.Tests;

internal static class DeclaredExtensions
{
    public const string DefaultDeclaringClass = "Billing.Api.InvoiceApi";

    public static Declared<T> DeclaredOn<T>(this T attribute, string declaringClass = DefaultDeclaringClass)
        => new(attribute, new DeclaringClass(declaringClass));
}

internal sealed class InMemoryContainerSource : IDeclarationSource
{
    private readonly Declared<ContainerAttribute>[] containers;
    private readonly Declared<DependsOnAttribute>[] dependencies;

    public InMemoryContainerSource(params ContainerAttribute[] containers)
        : this(containers, [])
    {
    }

    public InMemoryContainerSource(ContainerAttribute[] containers, params DependsOnAttribute[] dependencies)
        : this(
            [.. containers.Select(container => container.DeclaredOn())],
            [.. dependencies.Select(dependency => dependency.DeclaredOn())])
    {
    }

    public InMemoryContainerSource(Declared<ContainerAttribute>[] containers, Declared<DependsOnAttribute>[] dependencies)
    {
        this.containers = containers;
        this.dependencies = dependencies;
    }

    public IEnumerable<Declared<ContainerAttribute>> Containers() => containers;

    public IEnumerable<Declared<DependsOnAttribute>> Dependencies() => dependencies;
}

internal sealed class InMemoryContextDeclarationDslOutput : IContextDeclarationDslOutput
{
    public List<DeploymentUnitAggregate> Written { get; } = [];

    public void Write(DeploymentUnitAggregate unit) => Written.Add(unit);
}
