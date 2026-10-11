using Perpetua.Structurizr.Application;
using Perpetua.Structurizr.Domain;

namespace Perpetua.Structurizr.Tests;

internal static class AttributeClaimExtensions
{
    public const string DefaultDeclaringClass = "Billing.Api.InvoiceApi";

    public static ContainerClaim DeclaredOn(this ContainerAttribute attribute, string declaringClass = DefaultDeclaringClass)
        => new(attribute.Context, attribute.Name, new DeclaringClass(declaringClass));

    public static DependencyClaim DeclaredOn(this DependsOnAttribute attribute, string declaringClass = DefaultDeclaringClass)
        => new(attribute.Identifier, attribute.Context, attribute.Description, new DeclaringClass(declaringClass));
}

internal sealed class InMemoryContainerSource : IDeclarationSource
{
    private readonly ContainerClaim[] containers;
    private readonly DependencyClaim[] dependencies;

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

    public InMemoryContainerSource(ContainerClaim[] containers, DependencyClaim[] dependencies)
    {
        this.containers = containers;
        this.dependencies = dependencies;
    }

    public IEnumerable<ContainerClaim> Containers() => containers;

    public IEnumerable<DependencyClaim> Dependencies() => dependencies;
}

internal sealed class InMemoryDeploymentUnitDslOutput : IDeploymentUnitDslOutput
{
    public List<DeploymentUnitAggregate> Written { get; } = [];

    public void Write(DeploymentUnitAggregate unit) => Written.Add(unit);
}
