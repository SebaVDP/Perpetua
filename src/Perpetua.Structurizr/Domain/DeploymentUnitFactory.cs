using static Perpetua.Structurizr.Domain.ContainerDeclarationResult;

namespace Perpetua.Structurizr.Domain;

/// <summary>
/// Decides what a run produces from what its deployment unit declares:
/// none means nothing to document, exactly one container is documented together with its dependencies,
/// and more than one is refused because each deployment unit is documented by its own run.
/// Every problem found in the declarations is reported, so the user can resolve them in one go.
/// </summary>
public static class DeploymentUnitFactory
{
    public static ContainerDeclarationResult Create(
        IEnumerable<Declared<ContainerAttribute>> containers,
        IEnumerable<Declared<DependsOnAttribute>> dependencies)
    {
        var declared = dependencies.Select(DeclaredDependency.From).ToList();
        return containers.ToList() switch
        {
            [] => new NoContainersFound(),
            [var container] => Declare(container) switch
            {
                DeclarationGenerated built => AddDependencies(built.Declaration, declared),
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

    private static ContainerDeclarationResult Declare(Declared<ContainerAttribute> declared)
        => ContextName.Create(declared.Attribute.Context).Match<ContainerDeclarationResult>(
            context => ContainerName.Create(declared.Attribute.Name).Match<ContainerDeclarationResult>(
                name => new DeclarationGenerated(new DeploymentUnitAggregate(context, name)),
                () => new Rejected([Problem.InvalidContainerName(declared.Attribute.Name, declared.DeclaringClass)])),
            () => new Rejected([Problem.InvalidContextName(declared.Attribute.Context, declared.DeclaringClass)]));

    // Extends the unit with every valid dependency; every invalid or duplicate one is reported instead.
    private static ContainerDeclarationResult AddDependencies(DeploymentUnitAggregate unit, IEnumerable<DeclaredDependency> dependencies)
    {
        var problems = new List<Problem>();
        foreach (var dependency in dependencies)
        {
            switch (dependency)
            {
                case DeclaredDependency.Invalid invalid:
                    problems.Add(invalid.Problem);
                    break;
                case DeclaredDependency.Valid valid when unit.WithDependency(valid.ResolveIn(unit.ContextName))
                    is Result<DeploymentUnitAggregate>.Success { Value: var extended }:
                    unit = extended;
                    break;
                case DeclaredDependency.Valid duplicate:
                    problems.Add(Problem.DuplicateDependency(duplicate.Container, duplicate.DeclaringClass));
                    break;
            }
        }

        return problems.Count > 0 ? new Rejected(problems) : new DeclarationGenerated(unit);
    }
}
