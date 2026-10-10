namespace Perpetua.Structurizr.Application;

/// <summary>Where the container and dependency declarations of the deployment unit being documented come from.</summary>
public interface IDeclarationSource
{
    IEnumerable<ContainerAttribute> Containers();

    IEnumerable<DependsOnAttribute> Dependencies();
}
