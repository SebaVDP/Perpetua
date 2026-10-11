namespace Perpetua.Structurizr.Domain;

/// <summary>
/// Something in the declarations that makes the run fail and generate nothing; the user resolves it.
/// It has a stable code that identifies the kind of problem and a message that says what is wrong
/// and where, so the user can find it again. Each kind of problem is created by its own function.
/// </summary>
public sealed record Problem
{
    private Problem(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public string Code { get; }

    public string Message { get; }

    /// <summary>
    /// A deployment unit declared more than one container. Each deployment unit is documented by its own run.
    /// </summary>
    public static Problem OnlyOneContainerAllowed()
        => new("PSTRUCT.0001", "A deployment unit may declare only one container.");

    /// <summary>The container name cannot be used as a Structurizr identifier or as a folder name.</summary>
    public static Problem InvalidContainerName()
        => new("PSTRUCT.0002", "The container name is not a valid identifier.");

    /// <summary>The context cannot be used as a Structurizr identifier or as a folder name.</summary>
    public static Problem InvalidContextName()
        => new("PSTRUCT.0003", "The context is not a valid identifier.");

    /// <summary>A dependency identifier cannot be used as a Structurizr identifier.</summary>
    public static Problem InvalidDependencyIdentifier(string value, DeclaringClass declaringClass)
        => new("PSTRUCT.0004", $"Dependency identifier '{value}' on class '{declaringClass.FullName}' is not a valid identifier.");

    /// <summary>A dependency context cannot be used as a Structurizr identifier.</summary>
    public static Problem InvalidDependencyContext()
        => new("PSTRUCT.0005", "A dependency context is not a valid identifier.");

    /// <summary>
    /// The same dependency (target, context and description) is declared more than once on the container.
    /// Structurizr rejects a duplicate relationship.
    /// </summary>
    public static Problem DuplicateDependency()
        => new("PSTRUCT.0006", "A dependency is declared twice with the same target and description. Remove the duplicate.");
}
