using Perpetua;

namespace Perpetua.Structurizr.Domain;

using static ContainerDeclarationResult;

/// <summary>
/// Decides what a run produces from the containers its deployment unit declares:
/// none means nothing to document, exactly one is documented, and more than one is refused
/// because each deployment unit is documented by its own run.
/// </summary>
public sealed class ContainerDeclarationBuilder
{
    public ContainerDeclarationResult Build(IEnumerable<ContainerAttribute> containers)
        => containers.ToList() switch
        {
            [] => new NoContainersFound(),
            [var container] when Identifier.TryCreate(container.Name, out var name) =>
                new DeclarationGenerated(new ContextDeclaration(
                    container.Context,
                    new ContainerDeclaration(name))),
            [_] => new InvalidContainerName(),
            _ => new OnlyOneContainerAllowed()
        };
}
