using Perpetua.Structurizr.Application;
using Perpetua.Structurizr.Domain;

namespace Perpetua.Structurizr.Infrastructure;

public sealed class ContainerDslWriter(DirectoryInfo outputDirectory) : IContainerDslOutput
{
    public void Write(ContextDslFragment context)
    {
        var containerDeclarations = context.Containers
            .Select(container => $"container \"{container.Name}\"");
        var dsl = $"{string.Join('\n', containerDeclarations)}\n";
        File.WriteAllText(
            Path.Combine(outputDirectory.FullName, $"{context.ContextName}.dsl"),
            dsl);
    }
}
