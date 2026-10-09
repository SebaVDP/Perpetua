using Perpetua.Structurizr.Domain;
using static Perpetua.Structurizr.Domain.ContainerDslFragmentResult;

namespace Perpetua.Structurizr.Application;

/// <summary>
/// Documents the single container of a deployment unit as Structurizr DSL.
/// Output is only produced when the unit declares exactly one container.
/// </summary>
public sealed class GenerateContainerDslHandler(IContainerSource source, IContainerDslOutput output)
{
    public ContainerDslFragmentResult Handle()
    {
        var result = new ContainerDslFragmentBuilder().Build(source.Containers());
        if (result is FragmentGenerated generated)
        {
            output.Write(generated.Fragment);
        }

        return result;
    }
}
