using System.Reflection;
using Perpetua;

namespace Perpetua.Structurizr;

/// <summary>Discovers the containers declared by <see cref="ContainerAttribute"/> on types in a directory of assemblies.</summary>
public sealed class ContainerScanner
{
    /// <summary>Returns every container declared across the assemblies found in <paramref name="directory"/>.</summary>
    public IEnumerable<ContainerAttribute> Scan(string directory)
        => Directory.GetFiles(directory, "*.dll")
            .SelectMany(path => Assembly.LoadFrom(path).GetTypes())
            .Select(type => type.GetCustomAttribute<ContainerAttribute>())
            .OfType<ContainerAttribute>();
}
