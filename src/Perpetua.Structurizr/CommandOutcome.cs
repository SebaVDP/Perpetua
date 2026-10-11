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
        Rejected rejected => new(CommandExitCode.Failure, string.Join(Environment.NewLine, rejected.Problems.Select(Describe))),
        _ => throw new UnreachableException()
    };

    private static string Describe(Problem problem) => problem switch
    {
        Problem.OnlyOneContainerAllowed => "A deployment unit may declare only one container.",
        Problem.InvalidContainerName => "The container name is not a valid identifier.",
        Problem.InvalidContextName => "The context is not a valid identifier.",
        Problem.InvalidDependencyIdentifier => "A dependency identifier is not a valid identifier.",
        Problem.InvalidDependencyContext => "A dependency context is not a valid identifier.",
        Problem.DuplicateDependency => "A dependency is declared twice with the same target and description. Remove the duplicate.",
        _ => throw new UnreachableException()
    };
}
