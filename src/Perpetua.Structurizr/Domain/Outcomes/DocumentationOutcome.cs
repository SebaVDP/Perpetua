namespace Perpetua.Structurizr.Domain;

/// <summary>
/// What a run does with the deployment unit it reads.
/// It is exactly one of the nested cases; the private constructor keeps the set closed.
/// </summary>
public abstract record DocumentationOutcome
{
    private DocumentationOutcome()
    {
    }

    /// <summary>The deployment unit declares exactly one container, and this unit is what gets documented.</summary>
    public sealed record Documented(DeploymentUnitAggregate Unit) : DocumentationOutcome;

    /// <summary>The deployment unit declares no container, so there is nothing to document. This is not a failure.</summary>
    public sealed record NothingToDocument : DocumentationOutcome;

    /// <summary>The run fails and generates nothing, because of the problems the user has to resolve.</summary>
    public sealed record Rejected(IReadOnlyList<Problem> Problems) : DocumentationOutcome;
}
