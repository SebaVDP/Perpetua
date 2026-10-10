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
                if (ContainerName.Create(dependency.Identifier) is not Result<ContainerName>.Success { Value: var name })
                {
                    return new InvalidDependencyIdentifier();
                }

                dependencies.Add(new DependencyDeclaration(name, dependency.Context, dependency.Description));
            }

            var declaration = generated.Declaration with { Dependencies = dependencies };
            output.Write(declaration);
            return generated with { Declaration = declaration };
        }

        return result;
    }
}
