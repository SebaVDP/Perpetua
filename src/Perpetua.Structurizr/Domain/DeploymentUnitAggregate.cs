namespace Perpetua.Structurizr.Domain;

/// <summary>
/// A deployment unit as it is documented: its one container, the software-system context the container
/// belongs to, and the dependencies it declares. It is the aggregate root: a dependency can only be added
/// through it, so a relationship is never declared twice. Each deployment unit is documented by its own run.
/// </summary>
public sealed record DeploymentUnitAggregate(ContextName ContextName, ContainerName ContainerName)
{
    public IReadOnlyList<DependencyDeclaration> Dependencies { get; init; } = [];

    public QualifiedContainerName QualifiedName => new(ContextName, ContainerName);

    public IReadOnlyList<PublishedEvent> PublishedEvents { get; init; } = [];

    /// <summary>
    /// Adds a dependency. A relationship is identified by its target, context and description,
    /// so declaring the same one twice fails instead of producing a duplicate relationship.
    /// </summary>
    public Result<DeploymentUnitAggregate> WithDependency(DependencyDeclaration dependency)
        => Dependencies.Contains(dependency)
            ? new Result<DeploymentUnitAggregate>.Failure()
            : new Result<DeploymentUnitAggregate>.Success(this with { Dependencies = [.. Dependencies, dependency] });
}
