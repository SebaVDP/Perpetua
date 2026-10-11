namespace Perpetua.Structurizr.Domain;

/// <summary>
/// A dependency as the user declared it, checked for validity but not yet placed in a context:
/// when no context is declared, it defaults to the context of the container it sits in.
/// It is either valid, and then it can be resolved, or it carries the problem the user has to resolve.
/// </summary>
public sealed record DeclaredDependency
{
    private readonly Func<ContextName, DependencyDeclaration>? resolve;

    private DeclaredDependency(Declared<DependsOnAttribute> source, Func<ContextName, DependencyDeclaration>? resolve, Problem? problem)
    {
        Source = source;
        this.resolve = resolve;
        Problem = problem;
    }

    public Declared<DependsOnAttribute> Source { get; }

    public Problem? Problem { get; }

    public static DeclaredDependency From(Declared<DependsOnAttribute> declared)
    {
        var dependency = declared.Attribute;
        if (ContainerName.Create(dependency.Identifier) is not Result<ContainerName>.Success { Value: var name })
        {
            return new DeclaredDependency(declared, null, Problem.InvalidDependencyIdentifier(dependency.Identifier, declared.DeclaringClass));
        }

        if (dependency.Context is null)
        {
            return new DeclaredDependency(declared, defaultContext => new DependencyDeclaration(name, defaultContext, dependency.Description), null);
        }

        return ContextName.Create(dependency.Context) is Result<ContextName>.Success { Value: var context }
            ? new DeclaredDependency(declared, _ => new DependencyDeclaration(name, context, dependency.Description), null)
            : new DeclaredDependency(declared, null, Problem.InvalidDependencyContext(dependency.Context, declared.DeclaringClass));
    }

    public DependencyDeclaration? ResolveIn(ContextName defaultContext) => resolve?.Invoke(defaultContext);
}
