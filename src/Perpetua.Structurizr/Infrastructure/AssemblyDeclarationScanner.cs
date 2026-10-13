using System.Reflection;
using Perpetua.Structurizr.Application;
using Perpetua.Structurizr.Domain;

namespace Perpetua.Structurizr.Infrastructure;

public sealed class AssemblyDeclarationScanner(DirectoryInfo assemblyDirectory) : IDeclarationSource
{
    private readonly Lazy<Type[]> types = new(() => assemblyDirectory.GetFiles("*.dll")
        .SelectMany(assemblyFile => GetLoadableTypes(Assembly.LoadFrom(assemblyFile.FullName)))
        .ToArray());

    public IEnumerable<ContainerClaim> Containers()
        => Types().SelectMany(type => type.GetCustomAttributes<ContainerAttribute>()
            .Select(attribute => new ContainerClaim(attribute.Context, attribute.Name, new DeclaringClass(type.FullName!))));

    public IEnumerable<DependencyClaim> Dependencies()
        => Types().SelectMany(type => type.GetCustomAttributes<DependsOnAttribute>()
            .Select(attribute => new DependencyClaim(attribute.Name, attribute.Context, attribute.Description, new DeclaringClass(type.FullName!))));

    public IEnumerable<EventClaim> Events()
        => Types().SelectMany(type => type.GetCustomAttributes<EventAttribute>()
            .Select(attribute => new EventClaim(attribute.Name, new DeclaringClass(type.FullName!))));

    public IEnumerable<ConsumptionClaim> Consumptions()
        => Types().SelectMany(type => type.GetCustomAttributes<ConsumesAttribute>()
            .Select(attribute => new ConsumptionClaim(attribute.Publisher, attribute.EventName, attribute.Context, attribute.Description, new DeclaringClass(type.FullName!))));

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
