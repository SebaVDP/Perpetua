using Perpetua.Structurizr.Domain;
using static Perpetua.Structurizr.Domain.ContainerDeclarationResult;

namespace Perpetua.Structurizr.Application;

/// <summary>
/// Documents the single container of a deployment unit as Structurizr DSL.
/// Output is only produced when the unit declares exactly one container.
/// </summary>
public sealed class GenerateContainerDslHandler(IDeclarationSource source, IContextDeclarationDslOutput output)
{
    public ContainerDeclarationResult Handle()
    {
        var result = new ContainerDeclarationBuilder().Build(source.Containers().Select(container => container.Attribute));
        if (result is Rejected rejected)
        {
            return new Rejected([.. rejected.Problems, .. InvalidDependencyIdentifiers()]);
        }

        if (result is not DeclarationGenerated built)
        {
            return result;
        }

        var declared = source.Dependencies().Aggregate(new Declaring(built.Declaration, []), AddDependency);
        if (declared.Problems.Count > 0)
        {
            return new Rejected(declared.Problems);
        }

        output.Write(declared.Declaration);
        return new DeclarationGenerated(declared.Declaration);
    }

    private IEnumerable<Problem> InvalidDependencyIdentifiers()
        => source.Dependencies()
            .Where(dependency => ContainerName.Create(dependency.Attribute.Identifier) is not Result<ContainerName>.Success)
            .Select(dependency => Problem.InvalidDependencyIdentifier(dependency.Attribute.Identifier, dependency.DeclaringClass));

    // Extends the declaration with the dependency; otherwise keeps it and records the problem.
    private static Declaring AddDependency(Declaring declaring, Declared<DependsOnAttribute> declared)
    {
        var dependency = declared.Attribute;
        if (ContainerName.Create(dependency.Identifier) is not Result<ContainerName>.Success { Value: var name })
        {
            return declaring.With(Problem.InvalidDependencyIdentifier(dependency.Identifier, declared.DeclaringClass));
        }

        var context = declaring.Declaration.ContextName;
        if (dependency.Context is not null)
        {
            if (ContextName.Create(dependency.Context) is not Result<ContextName>.Success { Value: var created })
            {
                return declaring.With(Problem.InvalidDependencyContext());
            }

            context = created;
        }

        return declaring.Declaration.WithDependency(new DependencyDeclaration(name, context, dependency.Description))
            is Result<ContextDeclaration>.Success { Value: var extended }
            ? declaring with { Declaration = extended }
            : declaring.With(Problem.DuplicateDependency());
    }

    private sealed record Declaring(ContextDeclaration Declaration, IReadOnlyList<Problem> Problems)
    {
        public Declaring With(Problem problem) => this with { Problems = [.. Problems, problem] };
    }
}
