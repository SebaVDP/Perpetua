using Perpetua.Structurizr.Application;
using Perpetua.Structurizr.Domain;
using static Perpetua.Structurizr.Domain.ContainerDeclarationResult;

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
        var generated =         Assert.IsType<DeclarationGenerated>(result);
                var context = generated.Declaration;
        Assert.Same(context, Assert.Single(output.Written));
        Assert.Equal("ExampleSystem", context.ContextName);
        Assert.Equal(new ContainerDeclaration("Website"), context.Container);
    }

    [Theory]
    [InlineData("a.b")]
    [InlineData("a b")]
    [InlineData("é")]
    [InlineData("a/b")]
    [InlineData("a$b")]
    [InlineData("a:b")]
    [InlineData("-a")]
    [InlineData("")]
    public void Writes_nothing_when_the_container_name_is_not_a_valid_identifier(string name)
    {
        // Given
        var source = new InMemoryContainerSource(new ContainerAttribute("ExampleSystem", name));
        var output = new InMemoryContainerDslOutput();
        var handler = new GenerateContainerDslHandler(source, output);

        // When
        var result = handler.Handle();

        // Then
        Assert.IsType<InvalidContainerName>(result);
        Assert.Empty(output.Written);
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
