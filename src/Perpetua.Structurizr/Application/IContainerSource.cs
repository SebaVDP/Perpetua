namespace Perpetua.Structurizr.Application;

/// <summary>Where the containers of the deployment unit being documented come from.</summary>
public interface IContainerSource
{
    IEnumerable<ContainerAttribute> Containers();
}
