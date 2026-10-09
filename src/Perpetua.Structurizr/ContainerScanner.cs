using System.Reflection;
using Perpetua;

namespace Perpetua.Structurizr;

public sealed class ContainerScanner
{
    public IEnumerable<ContainerAttribute> Scan(DirectoryInfo assemblyDirectory)
        => assemblyDirectory.GetFiles("*.dll")
            .SelectMany(assemblyFile => Assembly.LoadFrom(assemblyFile.FullName).GetTypes())
            .Select(type => type.GetCustomAttribute<ContainerAttribute>())
            .OfType<ContainerAttribute>();
}
