using System.Diagnostics;
using Perpetua.Structurizr.Domain;
using static Perpetua.Structurizr.Domain.ContainerDeclarationResult;

namespace Perpetua.Structurizr;

/// <summary>What the command tells its caller after a run: an exit code and, when something went wrong, why.</summary>
internal sealed record CommandOutcome(CommandExitCode ExitCode, string? Message = null)
{
    public static CommandOutcome From(ContainerDeclarationResult result) => result switch
    {
        DeclarationGenerated or NoContainersFound => new(CommandExitCode.Success),
        Rejected { Problems: [var problem] } => From(problem),
        _ => throw new UnreachableException()
    };

    private static CommandOutcome From(Problem problem) => problem switch
    {
        Problem.OnlyOneContainerAllowed => new(CommandExitCode.OnlyOneContainerAllowed, "A deployment unit may declare only one container."),
        Problem.InvalidContainerName => new(CommandExitCode.InvalidContainerName, "The container name is not a valid identifier."),
        Problem.InvalidContextName => new(CommandExitCode.InvalidContextName, "The context is not a valid identifier."),
        Problem.InvalidDependencyIdentifier => new(CommandExitCode.InvalidDependencyIdentifier, "A dependency identifier is not a valid identifier."),
        Problem.InvalidDependencyContext => new(CommandExitCode.InvalidDependencyContext, "A dependency context is not a valid identifier."),
        Problem.DuplicateDependency => new(CommandExitCode.DuplicateDependency, "A dependency is declared twice with the same target and description. Remove the duplicate."),
        _ => throw new UnreachableException()
    };
}
