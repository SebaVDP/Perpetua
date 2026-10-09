namespace Perpetua.Structurizr;

public sealed class ContainerDslWriter
{
    public void Write(ContextDslFragment context, DirectoryInfo outputDirectory)
    {
        var containerDeclarations = context.Containers
            .Select(container => $"container \"{container.Name}\"");
        var dsl = $"{string.Join('\n', containerDeclarations)}\n";
        File.WriteAllText(
            Path.Combine(outputDirectory.FullName, $"{context.ContextName}.dsl"),
            dsl);
    }
}
