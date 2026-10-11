using Perpetua.Structurizr.Domain;

namespace Perpetua.Structurizr.Application;

/// <summary>Where the generated context declaration DSL goes.</summary>
public interface IDeploymentUnitDslOutput
{
    void Write(DeploymentUnitAggregate unit);
}
