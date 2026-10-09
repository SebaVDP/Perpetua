using System.Reflection;
using Perpetua.Structurizr.Application;

namespace Perpetua.Structurizr.Infrastructure;

public sealed class ContainerScanner(DirectoryInfo assemblyDirectory) : IContainerSource
{
    public IEnumerable<ContainerAttribute> Containers()
        => assemblyDirectory.GetFiles("*.dll")
            .SelectMany(assemblyFile => GetLoadableTypes(Assembly.LoadFrom(assemblyFile.FullName)))
            .Select(type => type.GetCustomAttribute<ContainerAttribute>())
            .OfType<ContainerAttribute>();

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException exception)
        {
            return exception.Types.OfType<Type>();
        }
    }
}
