using Perpetua.Structurizr.Application;
using Perpetua.Structurizr.Domain;

namespace Perpetua.Structurizr.Infrastructure;

public sealed class ContainerDslWriter(DirectoryInfo outputDirectory) : IContainerDslOutput
{
    public void Write(ContextDeclaration context)
    {
        var containersDirectory = Directory.CreateDirectory(
            Path.Combine(outputDirectory.FullName, context.ContextName, "containers"));
        File.WriteAllText(
            Path.Combine(containersDirectory.FullName, $"{context.Container.Name}.dsl"),
            $"{context.Container.Name} = container \"{context.Container.Name}\"\n");
    }
}
