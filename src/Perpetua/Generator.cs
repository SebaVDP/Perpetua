using Perpetua.Structurizr;

namespace Perpetua;

/// <summary>
/// Entry point that generates documentation artifacts from marked-up code.
/// The walking skeleton proves the wiring by producing a Structurizr workspace.
/// </summary>
public sealed class Generator
{
    private readonly WorkspaceDslWriter _writer = new();

    /// <summary>Generates a Structurizr <c>workspace.dsl</c> document.</summary>
    public string Generate() => _writer.Write();
}
