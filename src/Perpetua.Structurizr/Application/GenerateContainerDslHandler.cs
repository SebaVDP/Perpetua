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
        var result = source.Dependencies().Aggregate(
            new ContainerDeclarationBuilder().Build(source.Containers()),
            (outcome, dependency) => outcome is DeclarationGenerated generated
                ? AddDependency(generated.Declaration, dependency)
                : outcome);

        if (result is DeclarationGenerated final)
        {
            output.Write(final.Declaration);
        }

        return result;
    }

    // Extends the declaration with the dependency; otherwise returns the failure that stops the run.
    private static ContainerDeclarationResult AddDependency(ContextDeclaration declaration, DependsOnAttribute dependency)
    {
        if (ContainerName.Create(dependency.Identifier) is not Result<ContainerName>.Success { Value: var name })
        {
            return new InvalidDependencyIdentifier();
        }

        var context = declaration.ContextName;
        if (dependency.Context is not null)
        {
            if (ContextName.Create(dependency.Context) is not Result<ContextName>.Success { Value: var created })
            {
                return new InvalidDependencyContext();
            }

            context = created;
        }

        return declaration.WithDependency(new DependencyDeclaration(name, context, dependency.Description))
            is Result<ContextDeclaration>.Success { Value: var extended }
            ? new DeclarationGenerated(extended)
            : new DuplicateDependency();
    }
}
