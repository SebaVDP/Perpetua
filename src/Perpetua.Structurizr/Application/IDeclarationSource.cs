using Perpetua.Structurizr.Domain;

namespace Perpetua.Structurizr.Application;

/// <summary>Where the container and dependency declarations of the deployment unit being documented come from.</summary>
public interface IDeclarationSource
{
    IEnumerable<ContainerClaim> Containers();

    IEnumerable<DependencyClaim> Dependencies();

    IEnumerable<EventClaim> Events();

    IEnumerable<ConsumptionClaim> Consumptions();
}
