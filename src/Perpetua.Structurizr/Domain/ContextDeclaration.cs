namespace Perpetua.Structurizr.Domain;

/// <summary>
/// The container of a deployment unit together with the software-system context it belongs to.
/// A context holds exactly one container per run, because each deployment unit runs the tool separately.
/// </summary>
public sealed record ContextDeclaration(ContextName ContextName, ContainerDeclaration Container)
{
    public IReadOnlyList<DependencyDeclaration> Dependencies { get; init; } = [];

    /// <summary>
    /// Adds a dependency. A relationship is identified by its target, context and description,
    /// so declaring the same one twice fails instead of producing a duplicate relationship.
    /// </summary>
    public Result<ContextDeclaration> WithDependency(DependencyDeclaration dependency)
        => Dependencies.Any(existing => IsSameRelationship(existing, dependency))
            ? new Result<ContextDeclaration>.Failure()
            : new Result<ContextDeclaration>.Success(this with { Dependencies = [.. Dependencies, dependency] });

    // A dependency without a context targets the context of the container it sits in.
    private bool IsSameRelationship(DependencyDeclaration first, DependencyDeclaration second)
        => first.Container == second.Container
            && (first.Context ?? ContextName) == (second.Context ?? ContextName)
            && first.Description == second.Description;
}
