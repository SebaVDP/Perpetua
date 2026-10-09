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
        OnlyOneContainerAllowed => new(CommandExitCode.OnlyOneContainerAllowed, "A deployment unit may declare only one container."),
        InvalidContainerName => new(CommandExitCode.InvalidContainerName, "The container name is not a valid identifier."),
        InvalidContextName => new(CommandExitCode.InvalidContextName, "The context is not a valid identifier."),
        _ => throw new UnreachableException()
    };
}
