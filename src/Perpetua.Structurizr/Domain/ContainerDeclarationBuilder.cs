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
            [var container] => Declare(container),
            _ => new OnlyOneContainerAllowed()
        };

    private static ContainerDeclarationResult Declare(ContainerAttribute container)
        => ContextName.Create(container.Context).Match<ContainerDeclarationResult>(
            context => ContainerName.Create(container.Name).Match<ContainerDeclarationResult>(
                name => new DeclarationGenerated(new ContextDeclaration(context, new ContainerDeclaration(name))),
                () => new InvalidContainerName()),
            () => new InvalidContextName());
}
