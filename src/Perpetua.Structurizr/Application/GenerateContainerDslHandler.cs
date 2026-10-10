using Perpetua.Structurizr.Domain;
using static Perpetua.Structurizr.Domain.ContainerDeclarationResult;

namespace Perpetua.Structurizr.Application;

/// <summary>
/// Documents the single container of a deployment unit as Structurizr DSL.
/// Output is only produced when the unit declares exactly one container.
/// </summary>
public sealed class GenerateContainerDslHandler(IContainerSource source, IContainerDslOutput output)
{
    public ContainerDeclarationResult Handle()
    {
        var result = new ContainerDeclarationBuilder().Build(source.Containers());
        if (result is DeclarationGenerated generated)
        {
            var declaration = generated.Declaration with
            {
                Dependencies = source.Dependencies()
                    .Select(dependency => new DependencyDeclaration(
                        dependency.Identifier,
                        dependency.Context,
                        dependency.Description))
                    .ToArray()
            };
            output.Write(declaration);
            return generated with { Declaration = declaration };
        }

        return result;
    }
}
