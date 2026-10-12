using Perpetua.Structurizr.Domain;
using static Perpetua.Structurizr.Domain.DocumentationOutcome;

namespace Perpetua.Structurizr.Application;

/// <summary>
/// Documents the single container of a deployment unit as Structurizr DSL.
/// Output is only produced when the unit declares exactly one container.
/// </summary>
public sealed class GenerateContainerDslHandler(IDeclarationSource source, IDeploymentUnitDslOutput output)
{
    public DocumentationOutcome Handle()
    {
        var result = DeploymentUnitFactory.Create(source.Containers(), source.Dependencies(), source.Events(), source.Consumptions());
        if (result is Documented documented)
        {
            output.Write(documented.Unit);
        }

        return result;
    }
}
