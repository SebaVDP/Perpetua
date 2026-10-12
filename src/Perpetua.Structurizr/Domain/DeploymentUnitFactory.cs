using static Perpetua.Structurizr.Domain.DocumentationOutcome;

namespace Perpetua.Structurizr.Domain;

/// <summary>
/// Decides what a run produces from what its deployment unit declares:
/// none means nothing to document, exactly one container is documented together with its dependencies,
/// and more than one is refused because each deployment unit is documented by its own run.
/// Every problem found in the declarations is reported, so the user can resolve them in one go.
/// </summary>
public static class DeploymentUnitFactory
{
    public static DocumentationOutcome Create(
        IEnumerable<ContainerClaim> containers,
        IEnumerable<DependencyClaim> dependencies,
        IEnumerable<EventClaim> events)
    {
        var declared = dependencies.Select(DeclaredDependency.From).ToList();
        return containers.ToList() switch
        {
            [] => new NothingToDocument(),
            [var container] => Declare(container) switch
            {
                Documented built => AddDependencies(built.Unit, declared) switch
                {
                    Documented documented when DuplicateEvents(events) is [var duplicate, ..]
                        => new Rejected([duplicate]),
                    Documented documented when InvalidEvents(events) is [var invalid, ..]
                        => new Rejected([invalid]),
                    Documented documented => new Documented(documented.Unit with { PublishedEvents = PublishedBy(documented.Unit, events) }),
                    var other => other
                },
                Rejected rejected => new Rejected([.. rejected.Problems, .. DependencyProblems(declared)]),
                var other => other
            },
            var several => new Rejected([
                Problem.OnlyOneContainerAllowed([.. several.Select(container => container.DeclaringClass)]),
                .. DependencyProblems(declared)])
        };
    }

    private static List<Problem> DuplicateEvents(IEnumerable<EventClaim> events)
        => [.. events
            .GroupBy(claim => claim.Name)
            .SelectMany(group => group.Skip(1))
            .Select(claim => Problem.DuplicateEvent(claim.Name, claim.DeclaringClass))];

    private static List<Problem> InvalidEvents(IEnumerable<EventClaim> events)
        => [.. events
            .Where(claim => ContainerName.Create(claim.Name) is not Result<ContainerName>.Success)
            .Select(claim => Problem.InvalidEventName(claim.Name, claim.DeclaringClass))];

    private static IReadOnlyList<PublishedEvent> PublishedBy(DeploymentUnitAggregate unit, IEnumerable<EventClaim> events)
        => [.. events
            .Select(claim => ContainerName.Create($"{unit.ContainerName}_{claim.Name}"))
            .OfType<Result<ContainerName>.Success>()
            .Select(created => new PublishedEvent(created.Value))];

    private static IEnumerable<Problem> DependencyProblems(IEnumerable<DeclaredDependency> dependencies)
        => dependencies.OfType<DeclaredDependency.Invalid>().Select(invalid => invalid.Problem);

    private static DocumentationOutcome Declare(ContainerClaim claim)
        => ContextName.Create(claim.Context).Match<DocumentationOutcome>(
            context => ContainerName.Create(claim.Name).Match<DocumentationOutcome>(
                name => new Documented(new DeploymentUnitAggregate(context, name)),
                () => new Rejected([Problem.InvalidContainerName(claim.Name, claim.DeclaringClass)])),
            () => new Rejected([Problem.InvalidContextName(claim.Context, claim.DeclaringClass)]));

    // Extends the unit with every valid dependency; every invalid or duplicate one is reported instead.
    private static DocumentationOutcome AddDependencies(DeploymentUnitAggregate unit, IEnumerable<DeclaredDependency> dependencies)
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

        return problems.Count > 0 ? new Rejected(problems) : new Documented(unit);
    }
}
