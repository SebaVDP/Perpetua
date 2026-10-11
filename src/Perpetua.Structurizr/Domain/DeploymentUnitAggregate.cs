using static Perpetua.Structurizr.Domain.ContainerDeclarationResult;

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

    /// <summary>
    /// Decides what a run produces from what its deployment unit declares:
    /// none means nothing to document, exactly one container is documented together with its dependencies,
    /// and more than one is refused because each deployment unit is documented by its own run.
    /// Every problem found in the declarations is reported, so the user can resolve them in one go.
    /// </summary>
    public static ContainerDeclarationResult From(
        IEnumerable<Declared<ContainerAttribute>> containers,
        IEnumerable<Declared<DependsOnAttribute>> dependencies)
    {
        var declared = dependencies.Select(DeclaredDependency.From).ToList();
        return containers.ToList() switch
        {
            [] => new NoContainersFound(),
            [var container] => From(container) switch
            {
                DeclarationGenerated built => built.Declaration.WithDependencies(declared),
                Rejected rejected => new Rejected([.. rejected.Problems, .. DependencyProblems(declared)]),
                var other => other
            },
            var several => new Rejected([
                Problem.OnlyOneContainerAllowed([.. several.Select(container => container.DeclaringClass)]),
                .. DependencyProblems(declared)])
        };
    }

    private static IEnumerable<Problem> DependencyProblems(IEnumerable<DeclaredDependency> dependencies)
        => dependencies.OfType<DeclaredDependency.Invalid>().Select(invalid => invalid.Problem);

    private static ContainerDeclarationResult From(Declared<ContainerAttribute> declared)
        => ContextName.Create(declared.Attribute.Context).Match<ContainerDeclarationResult>(
            context => ContainerName.Create(declared.Attribute.Name).Match<ContainerDeclarationResult>(
                name => new DeclarationGenerated(new DeploymentUnitAggregate(context, name)),
                () => new Rejected([Problem.InvalidContainerName(declared.Attribute.Name, declared.DeclaringClass)])),
            () => new Rejected([Problem.InvalidContextName(declared.Attribute.Context, declared.DeclaringClass)]));

    /// <summary>Extends the declaration with every valid dependency; every invalid or duplicate one is reported instead.</summary>
    public ContainerDeclarationResult WithDependencies(IEnumerable<DeclaredDependency> dependencies)
    {
        var declaration = this;
        var problems = new List<Problem>();
        foreach (var dependency in dependencies)
        {
            switch (dependency)
            {
                case DeclaredDependency.Invalid invalid:
                    problems.Add(invalid.Problem);
                    break;
                case DeclaredDependency.Valid valid when declaration.WithDependency(valid.ResolveIn(ContextName))
                    is Result<DeploymentUnitAggregate>.Success { Value: var extended }:
                    declaration = extended;
                    break;
                case DeclaredDependency.Valid duplicate:
                    problems.Add(Problem.DuplicateDependency(duplicate.Container, duplicate.DeclaringClass));
                    break;
            }
        }

        return problems.Count > 0 ? new Rejected(problems) : new DeclarationGenerated(declaration);
    }

    /// <summary>
    /// Adds a dependency. A relationship is identified by its target, context and description,
    /// so declaring the same one twice fails instead of producing a duplicate relationship.
    /// </summary>
    public Result<DeploymentUnitAggregate> WithDependency(DependencyDeclaration dependency)
        => Dependencies.Contains(dependency)
            ? new Result<DeploymentUnitAggregate>.Failure()
            : new Result<DeploymentUnitAggregate>.Success(this with { Dependencies = [.. Dependencies, dependency] });
}
