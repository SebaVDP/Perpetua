using System.Reflection;
using Perpetua.Structurizr.Application;

namespace Perpetua.Structurizr.Infrastructure;

public sealed class AssemblyDeclarationScanner(DirectoryInfo assemblyDirectory) : IDeclarationSource
{
    private readonly Lazy<Type[]> types = new(() => assemblyDirectory.GetFiles("*.dll")
        .SelectMany(assemblyFile => GetLoadableTypes(Assembly.LoadFrom(assemblyFile.FullName)))
        .ToArray());

    public IEnumerable<Declared<ContainerAttribute>> Containers()
        => Types().SelectMany(type => type.GetCustomAttributes<ContainerAttribute>()
            .Select(attribute => new Declared<ContainerAttribute>(attribute, type.FullName!)));

    public IEnumerable<Declared<DependsOnAttribute>> Dependencies()
        => Types().SelectMany(type => type.GetCustomAttributes<DependsOnAttribute>()
            .Select(attribute => new Declared<DependsOnAttribute>(attribute, type.FullName!)));

    private IEnumerable<Type> Types()
        => types.Value;

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
