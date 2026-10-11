using Perpetua.Structurizr.Application;
using Perpetua.Structurizr.Domain;
using static Perpetua.Structurizr.Domain.ContainerDeclarationResult;
using static Perpetua.Structurizr.Tests.HandlerTestSupport;

namespace Perpetua.Structurizr.Tests;

public class GenerateContainerDslHandlerContainerTests
{
    [Fact]
    public void Writes_nothing_when_no_containers_are_found()
    {
        // Given
        var source = new InMemoryContainerSource();
        var output = new InMemoryContextDeclarationDslOutput();
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
        var output = new InMemoryContextDeclarationDslOutput();
        var handler = new GenerateContainerDslHandler(source, output);

        // When
        var result = handler.Handle();

        // Then
        var generated = Assert.IsType<DeclarationGenerated>(result);
        var context = generated.Declaration;
        Assert.Same(context, Assert.Single(output.Written));
        Assert.Equal("ExampleSystem", context.ContextName.Value);
        Assert.Equal("Website", context.ContainerName.Value);
    }

    [Fact]
    public void Writes_nothing_when_more_than_one_container_is_found()
    {
        // Given
        var source = new InMemoryContainerSource(
            [
                new ContainerAttribute("Billing", "Invoicing").DeclaredOn("Billing.Api.InvoiceApi"),
                new ContainerAttribute("Accounts", "Ledger").DeclaredOn("Accounting.Api.LedgerApi")
            ],
            []);
        var output = new InMemoryContextDeclarationDslOutput();
        var handler = new GenerateContainerDslHandler(source, output);

        // When
        var result = handler.Handle();

        // Then
        Assert.Equal(["PSTRUCT.0001: A deployment unit may declare only one container, but containers were found on classes 'Billing.Api.InvoiceApi', 'Accounting.Api.LedgerApi'."], Reported(result));
        Assert.Empty(output.Written);
    }

    [Theory]
    [InlineData("Invoice.Api")]
    [InlineData("Invoice Api")]
    [InlineData("Invoicé")]
    [InlineData("Invoice/Api")]
    [InlineData("Invoice$Api")]
    [InlineData("Invoice:Api")]
    [InlineData("-InvoiceApi")]
    [InlineData("")]
    public void Writes_nothing_when_the_container_name_is_not_a_valid_identifier(string name)
    {
        // Given
        var source = new InMemoryContainerSource(new ContainerAttribute("ExampleSystem", name));
        var output = new InMemoryContextDeclarationDslOutput();
        var handler = new GenerateContainerDslHandler(source, output);

        // When
        var result = handler.Handle();

        // Then
        Assert.Equal([$"PSTRUCT.0002: Container name '{name}' on class 'Billing.Api.InvoiceApi' is not a valid identifier."], Reported(result));
        Assert.Empty(output.Written);
    }

    [Theory]
    [InlineData("Example.System")]
    [InlineData("Example System")]
    [InlineData("É")]
    [InlineData("Example/System")]
    [InlineData("Example$System")]
    [InlineData("Example:System")]
    [InlineData("-Example")]
    [InlineData("")]
    public void Writes_nothing_when_the_context_is_not_a_valid_identifier(string context)
    {
        // Given
        var source = new InMemoryContainerSource(new ContainerAttribute(context, "Website"));
        var output = new InMemoryContextDeclarationDslOutput();
        var handler = new GenerateContainerDslHandler(source, output);

        // When
        var result = handler.Handle();

        // Then
        Assert.Equal([$"PSTRUCT.0003: Context '{context}' on class 'Billing.Api.InvoiceApi' is not a valid identifier."], Reported(result));
        Assert.Empty(output.Written);
    }
}
