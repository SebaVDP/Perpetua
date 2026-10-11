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
    public ContainerDeclarationResult Build(IEnumerable<Declared<ContainerAttribute>> containers)
        => containers.ToList() switch
        {
            [] => new NoContainersFound(),
            [var container] => Declare(container),
            _ => new Rejected([Problem.OnlyOneContainerAllowed([.. containers.Select(container => container.DeclaringClass)])])
        };

    private static ContainerDeclarationResult Declare(Declared<ContainerAttribute> declared)
        => ContextName.Create(declared.Attribute.Context).Match<ContainerDeclarationResult>(
            context => ContainerName.Create(declared.Attribute.Name).Match<ContainerDeclarationResult>(
                name => new DeclarationGenerated(new ContextDeclaration(context, new ContainerDeclaration(name))),
                () => new Rejected([Problem.InvalidContainerName(declared.Attribute.Name, declared.DeclaringClass)])),
            () => new Rejected([Problem.InvalidContextName(declared.Attribute.Context, declared.DeclaringClass)]));
}
