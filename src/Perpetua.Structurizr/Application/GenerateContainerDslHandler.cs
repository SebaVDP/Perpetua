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
        var result = new ContainerDeclarationBuilder().Build(source.Containers());
        if (result is DeclarationGenerated generated)
        {
            var dependencies = new List<DependencyDeclaration>();
            foreach (var dependency in source.Dependencies())
            {
                if (AddDependency(dependencies, dependency) is { } failure)
                {
                    return failure;
                }
            }

            var declaration = generated.Declaration with { Dependencies = dependencies };
            output.Write(declaration);
            return generated with { Declaration = declaration };
        }

        return result;
    }

    // Adds the dependency when it is valid; otherwise returns the failure that stops the run.
    private static ContainerDeclarationResult? AddDependency(
        List<DependencyDeclaration> dependencies,
        DependsOnAttribute dependency)
    {
        if (ContainerName.Create(dependency.Identifier) is not Result<ContainerName>.Success { Value: var name })
        {
            return new InvalidDependencyIdentifier();
        }

        ContextName? context = null;
        if (dependency.Context is not null)
        {
            if (ContextName.Create(dependency.Context) is not Result<ContextName>.Success { Value: var created })
            {
                return new InvalidDependencyContext();
            }

            context = created;
        }

        dependencies.Add(new DependencyDeclaration(name, context, dependency.Description));
        return null;
    }
}
