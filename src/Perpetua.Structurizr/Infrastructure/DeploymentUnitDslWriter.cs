using Perpetua.Structurizr.Application;
using Perpetua.Structurizr.Domain;

namespace Perpetua.Structurizr.Infrastructure;

public sealed class DeploymentUnitDslWriter(DirectoryInfo outputDirectory) : IDeploymentUnitDslOutput
{
    public void     Write(DeploymentUnitAggregate unit)
    {
        var name = unit.ContainerName;
        WriteFile(unit, "containers", name, $"{name} = container \"{name}\"\n" + string.Concat(unit.PublishedEvents.Select(Event)));
        WriteFile(
            unit,
            "relationships",
            name,
            string.Concat(unit.Dependencies.Select(dependency => Relationship(unit, dependency))));
    }

    private static string Event(PublishedEvent published)
        => $"{published.Name} = container \"{published.Name}\" \"\" \"\" \"Event\"\n";

    private static string Relationship(DeploymentUnitAggregate unit, DependencyDeclaration dependency)
    {
        var description = dependency.Description is null ? "" : $" \"{dependency.Description}\"";
        return $"{unit.QualifiedName} -> {dependency.Target}{description}\n";
    }

    private void WriteFile(DeploymentUnitAggregate unit, string folder, ContainerName name, string content)
    {
        var directory = Directory.CreateDirectory(
            Path.Combine(outputDirectory.FullName, unit.ContextName.Value, folder));
        File.WriteAllText(Path.Combine(directory.FullName, $"{name}.dsl"), content);
    }
}
