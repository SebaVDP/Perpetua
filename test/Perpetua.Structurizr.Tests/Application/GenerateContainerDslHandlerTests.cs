using Perpetua.Structurizr.Application;
using Perpetua.Structurizr.Domain;
using static Perpetua.Structurizr.Domain.ContainerDslFragmentResult;

namespace Perpetua.Structurizr.Tests;

public class GenerateContainerDslHandlerTests
{
    [Fact]
    public void Writes_nothing_when_no_containers_are_found()
    {
        // Given
        var source = new InMemoryContainerSource();
        var output = new InMemoryContainerDslOutput();
        var handler = new GenerateContainerDslHandler(source, output);

        // When
        var result = handler.Handle();

        // Then
        Assert.IsType<NoContainersFound>(result);
        Assert.Empty(output.Written);
    }

    [Fact]
    public void Writes_the_context_fragment_of_the_container_when_exactly_one_is_found()
    {
        // Given
        var source = new InMemoryContainerSource(new ContainerAttribute("ExampleSystem", "Website"));
        var output = new InMemoryContainerDslOutput();
        var handler = new GenerateContainerDslHandler(source, output);

        // When
        var result = handler.Handle();

        // Then
        var generated = Assert.IsType<FragmentGenerated>(result);
        var context = generated.Fragment;
        Assert.Same(context, Assert.Single(output.Written));
        Assert.Equal("ExampleSystem", context.ContextName);
        Assert.Equal(new ContainerDslFragment("Website"), context.Container);
    }

    [Fact]
    public void Writes_nothing_when_more_than_one_container_is_found()
    {
        // Given
        var source = new InMemoryContainerSource(
            new ContainerAttribute("Billing", "Invoicing"),
            new ContainerAttribute("Accounts", "Ledger"));
        var output = new InMemoryContainerDslOutput();
        var handler = new GenerateContainerDslHandler(source, output);

        // When
        var result = handler.Handle();

        // Then
        Assert.IsType<OnlyOneContainerAllowed>(result);
        Assert.Empty(output.Written);
    }
}
