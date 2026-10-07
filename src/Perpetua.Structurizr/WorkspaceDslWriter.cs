namespace Perpetua.Structurizr;

/// <summary>
/// Writes Structurizr workspace DSL. The walking-skeleton implementation emits
/// the canonical empty workspace that downstream tooling can render.
/// </summary>
public sealed class WorkspaceDslWriter
{
    /// <summary>Returns a valid, empty Structurizr <c>workspace.dsl</c> document.</summary>
    public string Write() =>
        "workspace {\n    model {\n    }\n    views {\n    }\n}\n";
}
