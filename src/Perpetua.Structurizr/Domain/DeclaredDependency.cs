namespace Perpetua.Structurizr.Domain;

/// <summary>
/// A dependency as the user declared it, checked for validity but not yet placed in a context:
/// when no context is declared, it defaults to the context of the container it sits in.
/// It is exactly one of the nested cases; the private constructor keeps the set closed.
/// </summary>
public abstract record DeclaredDependency
{
    private DeclaredDependency()
    {
    }

    public static DeclaredDependency From(Declared<DependsOnAttribute> declared)
    {
        var dependency = declared.Attribute;
        if (ContainerName.Create(dependency.Identifier) is not Result<ContainerName>.Success { Value: var name })
        {
            return new Invalid(Problem.InvalidDependencyIdentifier(dependency.Identifier, declared.DeclaringClass));
        }

        if (dependency.Context is null)
        {
            return new Valid(name, declared.DeclaringClass, defaultContext => new DependencyDeclaration(name, defaultContext, dependency.Description));
        }

        return ContextName.Create(dependency.Context) is Result<ContextName>.Success { Value: var context }
            ? new Valid(name, declared.DeclaringClass, _ => new DependencyDeclaration(name, context, dependency.Description))
            : new Invalid(Problem.InvalidDependencyContext(dependency.Context, declared.DeclaringClass));
    }

    /// <summary>The dependency can be placed in a context; it knows where it was declared, in case it turns out to be a duplicate.</summary>
    public sealed record Valid(ContainerName Container, DeclaringClass DeclaringClass, Func<ContextName, DependencyDeclaration> ResolveIn) : DeclaredDependency;

    /// <summary>The dependency cannot be used, and the problem tells the user what to resolve.</summary>
    public sealed record Invalid(Problem Problem) : DeclaredDependency;
}
