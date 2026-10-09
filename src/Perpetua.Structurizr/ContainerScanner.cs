using System.Reflection;
using Perpetua;

namespace Perpetua.Structurizr;

public sealed class ContainerScanner
{
    public IEnumerable<ContainerAttribute> Scan(DirectoryInfo assemblyDirectory)
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
