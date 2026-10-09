using Perpetua.Structurizr.Application;
using Perpetua.Structurizr.Domain;

namespace Perpetua.Structurizr.Infrastructure;

public sealed class ContainerDslWriter(DirectoryInfo outputDirectory) : IContainerDslOutput
{
    public void Write(ContextDslFragment context)
    {
        File.WriteAllText(
            Path.Combine(outputDirectory.FullName, $"{context.ContextName}.dsl"),
            $"container \"{context.Container.Name}\"\n");
    }
}
