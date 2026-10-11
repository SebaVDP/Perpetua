using System.Diagnostics;
using Perpetua.Structurizr.Domain;
using static Perpetua.Structurizr.Domain.DocumentationOutcome;

namespace Perpetua.Structurizr;

/// <summary>What the command tells its caller after a run: an exit code and, when something went wrong, why.</summary>
internal sealed record CommandOutcome(CommandExitCode ExitCode, string? Message = null)
{
    public static CommandOutcome From(DocumentationOutcome result) => result switch
    {
        Documented or NothingToDocument => new(CommandExitCode.Success),
        Rejected rejected => new(
            CommandExitCode.Failure,
            string.Join(Environment.NewLine, rejected.Problems.Select(problem => problem.ToString()))),
        _ => throw new UnreachableException()
    };
}
