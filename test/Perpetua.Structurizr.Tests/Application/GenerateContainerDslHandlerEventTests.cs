using Perpetua.Structurizr.Application;
using Perpetua.Structurizr.Domain;
using static Perpetua.Structurizr.Domain.DocumentationOutcome;
using static Perpetua.Structurizr.Tests.HandlerTestSupport;

namespace Perpetua.Structurizr.Tests;

public class GenerateContainerDslHandlerEventTests
{
    [Fact]
    public void Documents_the_event_the_publisher_declares_named_after_the_container()
    {
        // Given
        var source = new InMemoryContainerSource(
            [new ContainerAttribute("Billing", "InvoiceApi").DeclaredOn()],
            [],
            [new EventAttribute("OrderPlaced").DeclaredOn()]);
        var output = new InMemoryDeploymentUnitDslOutput();
        var handler = new GenerateContainerDslHandler(source, output);

        // When
        var result = handler.Handle();

        // Then
        var documented = Assert.IsType<Documented>(result);
        Assert.Same(documented.Unit, Assert.Single(output.Written));
        Assert.Equal(
            [Container("InvoiceApi_OrderPlaced")],
            documented.Unit.PublishedEvents.Select(published => published.Name));
    }

    [Fact]
    public void Writes_nothing_when_the_same_event_is_declared_twice_in_one_deployment_unit()
    {
        // Given
        var source = new InMemoryContainerSource(
            [new ContainerAttribute("Billing", "InvoiceApi").DeclaredOn()],
            [],
            [
                new EventAttribute("OrderPlaced").DeclaredOn("Billing.Api.InvoiceIssuer"),
                new EventAttribute("OrderPlaced").DeclaredOn("Billing.Api.InvoiceCanceller")
            ]);
        var output = new InMemoryDeploymentUnitDslOutput();
        var handler = new GenerateContainerDslHandler(source, output);

        // When
        var result = handler.Handle();

        // Then
        Assert.Equal(["PSTRUCT.0007: Event 'OrderPlaced' on class 'Billing.Api.InvoiceCanceller' is declared twice in the same deployment unit. Remove the duplicate."], Reported(result));
        Assert.Empty(output.Written);
    }
}
