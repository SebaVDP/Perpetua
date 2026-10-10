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
            string.Concat(context.Dependencies.Select(dependency =>
                $"{context.ContextName}.{name} -> {dependency.Context ?? context.ContextName}.{dependency.Identifier}\n")));
    }

    private void WriteFile(ContextDeclaration context, string folder, Identifier name, string content)
    {
        var directory = Directory.CreateDirectory(
            Path.Combine(outputDirectory.FullName, context.ContextName, folder));
        File.WriteAllText(Path.Combine(directory.FullName, $"{name}.dsl"), content);
    }
}
