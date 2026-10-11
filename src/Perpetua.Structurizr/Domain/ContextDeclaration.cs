using static Perpetua.Structurizr.Domain.ContainerDeclarationResult;

namespace Perpetua.Structurizr.Domain;

/// <summary>
/// The container of a deployment unit together with the software-system context it belongs to.
/// A context holds exactly one container per run, because each deployment unit runs the tool separately.
/// </summary>
public sealed record ContextDeclaration(ContextName ContextName, ContainerName ContainerName)
{
    public IReadOnlyList<DependencyDeclaration> Dependencies { get; init; } = [];

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
        => dependencies.Select(dependency => dependency.Problem).OfType<Problem>();

    private static ContainerDeclarationResult From(Declared<ContainerAttribute> declared)
        => ContextName.Create(declared.Attribute.Context).Match<ContainerDeclarationResult>(
            context => ContainerName.Create(declared.Attribute.Name).Match<ContainerDeclarationResult>(
                name => new DeclarationGenerated(new ContextDeclaration(context, name)),
                () => new Rejected([Problem.InvalidContainerName(declared.Attribute.Name, declared.DeclaringClass)])),
            () => new Rejected([Problem.InvalidContextName(declared.Attribute.Context, declared.DeclaringClass)]));

    /// <summary>Extends the declaration with every valid dependency; every invalid or duplicate one is reported instead.</summary>
    public ContainerDeclarationResult WithDependencies(IEnumerable<DeclaredDependency> dependencies)
    {
        var declaration = this;
        var problems = new List<Problem>();
        foreach (var dependency in dependencies)
        {
            if (dependency.ResolveIn(ContextName) is not { } resolved)
            {
                problems.Add(dependency.Problem!);
            }
            else if (declaration.WithDependency(resolved) is Result<ContextDeclaration>.Success { Value: var extended })
            {
                declaration = extended;
            }
            else
            {
                problems.Add(Problem.DuplicateDependency(dependency.Source.Attribute.Identifier, dependency.Source.DeclaringClass));
            }
        }

        return problems.Count > 0 ? new Rejected(problems) : new DeclarationGenerated(declaration);
    }

    /// <summary>
    /// Adds a dependency. A relationship is identified by its target, context and description,
    /// so declaring the same one twice fails instead of producing a duplicate relationship.
    /// </summary>
    public Result<ContextDeclaration> WithDependency(DependencyDeclaration dependency)
        => Dependencies.Contains(dependency)
            ? new Result<ContextDeclaration>.Failure()
            : new Result<ContextDeclaration>.Success(this with { Dependencies = [.. Dependencies, dependency] });
}
