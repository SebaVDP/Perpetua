using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using Perpetua.Structurizr.Application;
using Perpetua.Structurizr.Domain;

namespace Perpetua.Structurizr.Infrastructure;

/// <summary>
/// Reads the declarations from the metadata of the assemblies, without loading their types.
/// A class whose base type or attributes come from a framework that is not available to the tool,
/// such as an ASP.NET Core controller, is therefore found like any other class.
/// </summary>
/// <remarks>
/// Loading the types with reflection fails for a class whose base type cannot be resolved,
/// and its declarations would silently disappear. The metadata can always be read.
/// </remarks>
public sealed class AssemblyDeclarationScanner(DirectoryInfo assemblyDirectory) : IDeclarationSource
{
    private static readonly Dictionary<string, Type> DeclarationAttributes =
        new[] { typeof(ContainerAttribute), typeof(DependsOnAttribute), typeof(PublishesAttribute), typeof(ConsumesAttribute) }
            .ToDictionary(type => type.FullName!);

    private readonly Lazy<List<Declaration>> declarations = new(() => [.. assemblyDirectory.GetFiles("*.dll").SelectMany(Read)]);

    public IEnumerable<ContainerClaim> Containers()
        => Declared<ContainerAttribute>()
            .Select(declared => new ContainerClaim(declared.Attribute.Context, declared.Attribute.Name, declared.DeclaringClass));

    public IEnumerable<DependencyClaim> Dependencies()
        => Declared<DependsOnAttribute>()
            .Select(declared => new DependencyClaim(declared.Attribute.Name, declared.Attribute.Context, declared.Attribute.Description, declared.DeclaringClass));

    public IEnumerable<EventClaim> Events()
        => Declared<PublishesAttribute>()
            .Select(declared => new EventClaim(declared.Attribute.Name, declared.DeclaringClass));

    public IEnumerable<ConsumptionClaim> Consumptions()
        => Declared<ConsumesAttribute>()
            .Select(declared => new ConsumptionClaim(declared.Attribute.Publisher, declared.Attribute.EventName, declared.Attribute.Context, declared.Attribute.Description, declared.DeclaringClass));

    private IEnumerable<(T Attribute, DeclaringClass DeclaringClass)> Declared<T>() where T : Attribute
        => declarations.Value
            .Where(declaration => declaration.Attribute is T)
            .Select(declaration => ((T)declaration.Attribute, declaration.DeclaringClass));

    private static List<Declaration> Read(FileInfo assemblyFile)
    {
        using var stream = assemblyFile.OpenRead();
        using var peReader = new PEReader(stream);
        var reader = peReader.GetMetadataReader();

        return [.. reader.TypeDefinitions.SelectMany(handle =>
        {
            var type = reader.GetTypeDefinition(handle);
            var declaringClass = new DeclaringClass(FullNameOf(reader, type));

            return type.GetCustomAttributes()
                .Select(reader.GetCustomAttribute)
                .Where(attribute => IsDeclarationAttribute(reader, attribute))
                .Select(attribute => DeclarationOf(reader, attribute, declaringClass));
        })];
    }

    /// <summary>
    /// Reads the positional string arguments straight from the attribute blob:
    /// a two-byte prolog, then one serialized string per constructor parameter,
    /// and builds the attribute through its constructor.
    /// </summary>
    private static Declaration DeclarationOf(MetadataReader reader, CustomAttribute attribute, DeclaringClass declaringClass)
    {
        var type = TypeOf(reader, attribute);
        var blob = reader.GetBlobReader(attribute.Value);
        blob.ReadUInt16();
        // Stryker disable once Linq : equivalent mutant. Every declaration attribute has exactly one constructor,
        // so Single() and SingleOrDefault() behave identically for the types the scanner knows.
        var arguments = type.GetConstructors().Single().GetParameters()
            .Select(_ => (object?)blob.ReadSerializedString())
            .ToArray();

        return new Declaration((Attribute)Activator.CreateInstance(type, arguments)!, declaringClass);
    }

    /// <summary>
    /// Recognizes a declaration attribute by the full name of its type in the metadata,
    /// so the assembly that defines the attribute never has to be resolved.
    /// </summary>
    private static bool IsDeclarationAttribute(MetadataReader reader, CustomAttribute attribute)
    {
        if (attribute.Constructor.Kind != HandleKind.MemberReference)
        {
            return false;
        }

        var constructor = reader.GetMemberReference((MemberReferenceHandle)attribute.Constructor);
        return constructor.Parent.Kind == HandleKind.TypeReference
            && DeclarationAttributes.ContainsKey(FullNameOf(reader, (TypeReferenceHandle)constructor.Parent));
    }

    private static Type TypeOf(MetadataReader reader, CustomAttribute attribute)
    {
        var constructor = reader.GetMemberReference((MemberReferenceHandle)attribute.Constructor);
        return DeclarationAttributes[FullNameOf(reader, (TypeReferenceHandle)constructor.Parent)];
    }

    private static string FullNameOf(MetadataReader reader, TypeReferenceHandle handle)
    {
        var attributeType = reader.GetTypeReference(handle);
        return $"{reader.GetString(attributeType.Namespace)}.{reader.GetString(attributeType.Name)}";
    }

    private static string FullNameOf(MetadataReader reader, TypeDefinition type)
    {
        var name = reader.GetString(type.Name);
        var declaringType = type.GetDeclaringType();
        if (!declaringType.IsNil)
        {
            return $"{FullNameOf(reader, reader.GetTypeDefinition(declaringType))}+{name}";
        }

        var ns = reader.GetString(type.Namespace);
        return ns.Length == 0 ? name : $"{ns}.{name}";
    }

    private sealed record Declaration(Attribute Attribute, DeclaringClass DeclaringClass);
}
