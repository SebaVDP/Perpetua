using Perpetua.Structurizr.Domain;

namespace Perpetua.Structurizr.Application;

public sealed class GenerateContainerDslHandler(IContainerSource source, IContainerDslOutput output)
{
    public ContainerDslFragmentsResult Handle()
    {
        var result = new ContainerDslFragmentsBuilder().Build(source.Containers());
        if (result is ContextDslFragment context)
        {
            output.Write(context);
        }

        return result;
    }
}
