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
        var result = DeploymentUnitAggregate.From(source.Containers(), source.Dependencies());
        if (result is DeclarationGenerated generated)
        {
            output.Write(generated.Declaration);
        }

        return result;
    }
}
