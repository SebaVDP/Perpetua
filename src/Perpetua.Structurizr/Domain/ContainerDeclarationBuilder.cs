using System.Text.RegularExpressions;
using Perpetua;

namespace Perpetua.Structurizr.Domain;

using static ContainerDeclarationResult;

/// <summary>
/// Decides what a run produces from the containers its deployment unit declares:
/// none means nothing to document, exactly one is documented, and more than one is refused
/// because each deployment unit is documented by its own run.
/// </summary>
public sealed partial class ContainerDeclarationBuilder
{
    // Structurizr only accepts these characters in an identifier and rejects a leading '-'.
    [GeneratedRegex("^[A-Za-z0-9_][A-Za-z0-9_-]*$")]
    private static partial Regex ValidIdentifier { get; }

    public ContainerDeclarationResult Build(IEnumerable<ContainerAttribute> containers)
        => containers.ToList() switch
        {
            [] => new NoContainersFound(),
            [var container] when !ValidIdentifier.IsMatch(container.Name) => new InvalidContainerName(),
            [var container] => new DeclarationGenerated(new ContextDeclaration(
                container.Context,
                new ContainerDeclaration(container.Name))),
            _ => new OnlyOneContainerAllowed()
        };
}
