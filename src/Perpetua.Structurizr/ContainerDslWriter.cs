namespace Perpetua.Structurizr;

public sealed class ContainerDslWriter
{
    public void Write(ContainerDslFragments fragments, DirectoryInfo outputDirectory)
    {
        foreach (var context in fragments.Contexts)
        {
            var containerDeclarations = context.Containers
                .Select(container => $"container \"{container.Name}\"");
            var dsl = $"{string.Join('\n', containerDeclarations)}\n";
            File.WriteAllText(
                Path.Combine(outputDirectory.FullName, $"{context.ContextName}.dsl"),
                dsl);
        }
    }
}
