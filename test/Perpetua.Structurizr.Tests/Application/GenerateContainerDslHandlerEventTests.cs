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
    public void Documents_a_dependency_on_the_event_of_the_publisher_a_consumer_names()
    {
        // Given
        var source = new InMemoryContainerSource(
            [new ContainerAttribute("Shipping", "ShippingApi").DeclaredOn()],
            [],
            [],
            [new ConsumesAttribute("InvoiceApi", "OrderPlaced", "Billing").DeclaredOn("Shipping.Api.OrderListener")]);
        var output = new InMemoryDeploymentUnitDslOutput();
        var handler = new GenerateContainerDslHandler(source, output);

        // When
        var result = handler.Handle();

        // Then
        var documented = Assert.IsType<Documented>(result);
        Assert.Equal(
            [new DependencyDeclaration(Container("InvoiceApi_OrderPlaced"), Context("Billing"), null)],
            documented.Unit.Dependencies);
    }

    [Fact]
    public void Writes_nothing_when_a_consumed_event_name_is_not_a_valid_identifier()
    {
        // Given
        var source = new InMemoryContainerSource(
            [new ContainerAttribute("Shipping", "ShippingApi").DeclaredOn()],
            [],
            [],
            [new ConsumesAttribute("InvoiceApi", "Order.Placed", "Billing").DeclaredOn("Shipping.Api.OrderListener")]);
        var output = new InMemoryDeploymentUnitDslOutput();
        var handler = new GenerateContainerDslHandler(source, output);

        // When
        var result = handler.Handle();

        // Then
        Assert.Equal(["PSTRUCT.0009: Consumed event 'Order.Placed' of 'InvoiceApi' on class 'Shipping.Api.OrderListener' is not a valid identifier."], Reported(result));
        Assert.Empty(output.Written);
    }

    [Fact]
    public void Documents_the_same_event_name_in_different_deployment_units_under_different_container_names()
    {
        // Given
        var billing = new InMemoryContainerSource(
            [new ContainerAttribute("Billing", "InvoiceApi").DeclaredOn()],
            [],
            [new EventAttribute("OrderPlaced").DeclaredOn()]);
        var shipping = new InMemoryContainerSource(
            [new ContainerAttribute("Shipping", "ShippingApi").DeclaredOn()],
            [],
            [new EventAttribute("OrderPlaced").DeclaredOn()]);

        // When
        var billingResult = new GenerateContainerDslHandler(billing, new InMemoryDeploymentUnitDslOutput()).Handle();
        var shippingResult = new GenerateContainerDslHandler(shipping, new InMemoryDeploymentUnitDslOutput()).Handle();

        // Then
        Assert.Equal(
            [Container("InvoiceApi_OrderPlaced")],
            Assert.IsType<Documented>(billingResult).Unit.PublishedEvents.Select(published => published.Name));
        Assert.Equal(
            [Container("ShippingApi_OrderPlaced")],
            Assert.IsType<Documented>(shippingResult).Unit.PublishedEvents.Select(published => published.Name));
    }

    [Fact]
    public void Writes_nothing_when_an_event_name_is_not_a_valid_identifier()
    {
        // Given
        var source = new InMemoryContainerSource(
            [new ContainerAttribute("Billing", "InvoiceApi").DeclaredOn()],
            [],
            [new EventAttribute("Order.Placed").DeclaredOn("Billing.Api.InvoiceIssuer")]);
        var output = new InMemoryDeploymentUnitDslOutput();
        var handler = new GenerateContainerDslHandler(source, output);

        // When
        var result = handler.Handle();

        // Then
        Assert.Equal(["PSTRUCT.0008: Event name 'Order.Placed' on class 'Billing.Api.InvoiceIssuer' is not a valid identifier."], Reported(result));
        Assert.Empty(output.Written);
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
