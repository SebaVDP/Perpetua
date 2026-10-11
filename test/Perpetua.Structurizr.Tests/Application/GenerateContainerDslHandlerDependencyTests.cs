using Perpetua.Structurizr.Application;
using Perpetua.Structurizr.Domain;
using static Perpetua.Structurizr.Domain.ContainerDeclarationResult;
using static Perpetua.Structurizr.Tests.HandlerTestSupport;

namespace Perpetua.Structurizr.Tests;

public class GenerateContainerDslHandlerDependencyTests
{
    [Fact]
    public void Writes_a_relationship_to_the_container_context_when_dependency_has_no_context()
    {
        // Given
        var source = new InMemoryContainerSource(
            [new ContainerAttribute("Billing", "InvoiceApi")],
            new DependsOnAttribute("PaymentGateway"));
        var output = new InMemoryContextDeclarationDslOutput();
        var handler = new GenerateContainerDslHandler(source, output);

        // When
        var result = handler.Handle();

        // Then
        var generated = Assert.IsType<DeclarationGenerated>(result);
        Assert.Same(generated.Declaration, Assert.Single(output.Written));
        Assert.Equal(new DependencyDeclaration(Container("PaymentGateway"), Context("Billing"), null), Assert.Single(generated.Declaration.Dependencies));
    }

    [Fact]
    public void Writes_a_relationship_to_the_specified_context()
    {
        // Given
        var source = new InMemoryContainerSource(
            [new ContainerAttribute("Billing", "InvoiceApi")],
            new DependsOnAttribute("LedgerApi", "Accounting"));
        var output = new InMemoryContextDeclarationDslOutput();
        var handler = new GenerateContainerDslHandler(source, output);

        // When
        var result = handler.Handle();

        // Then
        var generated = Assert.IsType<DeclarationGenerated>(result);
        Assert.Same(generated.Declaration, Assert.Single(output.Written));
        Assert.Equal(new DependencyDeclaration(Container("LedgerApi"), Context("Accounting"), null), Assert.Single(generated.Declaration.Dependencies));
    }

    [Fact]
    public void Writes_both_relationships_when_the_same_container_is_a_dependency_in_two_contexts()
    {
        // Given
        var source = new InMemoryContainerSource(
            [new ContainerAttribute("Billing", "InvoiceApi")],
            new DependsOnAttribute("NotificationService", "Accounting"),
            new DependsOnAttribute("NotificationService", "Shipping"));
        var output = new InMemoryContextDeclarationDslOutput();
        var handler = new GenerateContainerDslHandler(source, output);

        // When
        var result = handler.Handle();

        // Then
        var generated = Assert.IsType<DeclarationGenerated>(result);
        Assert.Equal(
            [
                new DependencyDeclaration(Container("NotificationService"), Context("Accounting"), null),
                new DependencyDeclaration(Container("NotificationService"), Context("Shipping"), null)
            ],
            generated.Declaration.Dependencies);
    }
}
