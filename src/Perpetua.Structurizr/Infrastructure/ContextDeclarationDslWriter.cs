using Perpetua.Structurizr.Application;
using Perpetua.Structurizr.Domain;

namespace Perpetua.Structurizr.Infrastructure;

public sealed class ContextDeclarationDslWriter(DirectoryInfo outputDirectory) : IContextDeclarationDslOutput
{
    public void Write(ContextDeclaration context)
    {
        var name = context.Container.Name;
        WriteFile(context, "containers", name, $"{name} = container \"{name}\"\n");
        WriteFile(
            context,
            "relationships",
            name,
            string.Concat(context.Dependencies.Select(dependency => Relationship(context, dependency))));
    }

    private static string Relationship(ContextDeclaration context, DependencyDeclaration dependency)
    {
        var source = $"{context.ContextName}.{context.Container.Name}";
        var target = $"{dependency.Context ?? context.ContextName.Value}.{dependency.Identifier}";
        var description = dependency.Description is null ? "" : $" \"{dependency.Description}\"";
        return $"{source} -> {target}{description}\n";
    }

    private void WriteFile(ContextDeclaration context, string folder, ContainerName name, string content)
    {
        var directory = Directory.CreateDirectory(
            Path.Combine(outputDirectory.FullName, context.ContextName.Value, folder));
        File.WriteAllText(Path.Combine(directory.FullName, $"{name}.dsl"), content);
    }
}
