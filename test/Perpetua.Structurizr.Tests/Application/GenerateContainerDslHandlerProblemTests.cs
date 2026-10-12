using Perpetua.Structurizr.Application;
using static Perpetua.Structurizr.Tests.HandlerTestSupport;

namespace Perpetua.Structurizr.Tests;

public class GenerateContainerDslHandlerProblemTests
{
    [Fact]
    public void Writes_nothing_when_a_dependency_identifier_is_not_a_valid_identifier()
    {
        // Given
        var source = new InMemoryContainerSource(
            [new ContainerAttribute("Billing", "InvoiceApi").DeclaredOn()],
            [new DependsOnAttribute("Payment.Gateway").DeclaredOn("Billing.Api.PaymentClient")]);
        var output = new InMemoryDeploymentUnitDslOutput();
        var handler = new GenerateContainerDslHandler(source, output);

        // When
        var result = handler.Handle();

        // Then
        Assert.Equal(["PSTRUCT.0004: Dependency identifier 'Payment.Gateway' on class 'Billing.Api.PaymentClient' is not a valid identifier."], Reported(result));
        Assert.Empty(output.Written);
    }

    [Fact]
    public void Writes_nothing_when_a_dependency_context_is_not_a_valid_identifier()
    {
        // Given
        var source = new InMemoryContainerSource(
            [new ContainerAttribute("Billing", "InvoiceApi").DeclaredOn()],
            [new DependsOnAttribute("LedgerApi", "Accounting.Ledger").DeclaredOn("Billing.Api.LedgerClient")]);
        var output = new InMemoryDeploymentUnitDslOutput();
        var handler = new GenerateContainerDslHandler(source, output);

        // When
        var result = handler.Handle();

        // Then
        Assert.Equal(["PSTRUCT.0005: Dependency context 'Accounting.Ledger' on class 'Billing.Api.LedgerClient' is not a valid identifier."], Reported(result));
        Assert.Empty(output.Written);
    }

    [Fact]
    public void Reports_every_invalid_dependency_when_several_are_declared()
    {
        // Given
        var source = new InMemoryContainerSource(
            [new ContainerAttribute("Billing", "InvoiceApi").DeclaredOn()],
            [
                new DependsOnAttribute("Payment.Gateway").DeclaredOn("Billing.Api.PaymentClient"),
                new DependsOnAttribute("LedgerApi", "Accounting.Ledger").DeclaredOn("Billing.Api.LedgerClient")
            ]);
        var output = new InMemoryDeploymentUnitDslOutput();
        var handler = new GenerateContainerDslHandler(source, output);

        // When
        var result = handler.Handle();

        // Then
        Assert.Equal(
            [
                "PSTRUCT.0004: Dependency identifier 'Payment.Gateway' on class 'Billing.Api.PaymentClient' is not a valid identifier.",
                "PSTRUCT.0005: Dependency context 'Accounting.Ledger' on class 'Billing.Api.LedgerClient' is not a valid identifier."
            ],
            Reported(result));
        Assert.Empty(output.Written);
    }

    [Fact]
    public void Writes_nothing_when_the_same_dependency_is_declared_twice()
    {
        // Given
        var source = new InMemoryContainerSource(
            [new ContainerAttribute("Billing", "InvoiceApi").DeclaredOn()],
            [
                new DependsOnAttribute("LedgerApi", "Accounting", "Posts invoices").DeclaredOn("Billing.Api.LedgerClient"),
                new DependsOnAttribute("LedgerApi", "Accounting", "Posts invoices").DeclaredOn("Billing.Api.InvoiceJob")
            ]);
        var output = new InMemoryDeploymentUnitDslOutput();
        var handler = new GenerateContainerDslHandler(source, output);

        // When
        var result = handler.Handle();

        // Then
        Assert.Equal(["PSTRUCT.0006: Dependency 'LedgerApi' on class 'Billing.Api.InvoiceJob' is declared twice with the same context and description. Remove the duplicate."], Reported(result));
        Assert.Empty(output.Written);
    }

    [Fact]
    public void Writes_nothing_when_a_dependency_is_declared_twice_with_and_without_the_container_context()
    {
        // Given
        var source = new InMemoryContainerSource(
            [new ContainerAttribute("Billing", "InvoiceApi").DeclaredOn()],
            [
                new DependsOnAttribute("PaymentGateway").DeclaredOn("Billing.Api.PaymentClient"),
                new DependsOnAttribute("PaymentGateway", "Billing").DeclaredOn("Billing.Api.InvoiceJob")
            ]);
        var output = new InMemoryDeploymentUnitDslOutput();
        var handler = new GenerateContainerDslHandler(source, output);

        // When
        var result = handler.Handle();

        // Then
        Assert.Equal(["PSTRUCT.0006: Dependency 'PaymentGateway' on class 'Billing.Api.InvoiceJob' is declared twice with the same context and description. Remove the duplicate."], Reported(result));
        Assert.Empty(output.Written);
    }

    [Fact]
    public void Reports_an_invalid_dependency_context_together_with_an_invalid_container_name()
    {
        // Given
        var source = new InMemoryContainerSource(
            [new ContainerAttribute("Billing", "Invoice.Api").DeclaredOn()],
            [new DependsOnAttribute("LedgerApi", "Accounting.Ledger").DeclaredOn("Billing.Api.LedgerClient")]);
        var output = new InMemoryDeploymentUnitDslOutput();
        var handler = new GenerateContainerDslHandler(source, output);

        // When
        var result = handler.Handle();

        // Then
        Assert.Equal(
            [
                "PSTRUCT.0002: Container name 'Invoice.Api' on class 'Billing.Api.InvoiceApi' is not a valid identifier.",
                "PSTRUCT.0005: Dependency context 'Accounting.Ledger' on class 'Billing.Api.LedgerClient' is not a valid identifier."
            ],
            Reported(result));
        Assert.Empty(output.Written);
    }

    [Fact]
    public void Reports_only_the_invalid_container_name_when_the_dependencies_are_valid()
    {
        // Given
        var source = new InMemoryContainerSource(
            [new ContainerAttribute("Billing", "Invoice.Api").DeclaredOn()],
            [
                new DependsOnAttribute("PaymentGateway").DeclaredOn("Billing.Api.PaymentClient"),
                new DependsOnAttribute("LedgerApi", "Accounting").DeclaredOn("Billing.Api.LedgerClient")
            ]);
        var output = new InMemoryDeploymentUnitDslOutput();
        var handler = new GenerateContainerDslHandler(source, output);

        // When
        var result = handler.Handle();

        // Then
        Assert.Equal(["PSTRUCT.0002: Container name 'Invoice.Api' on class 'Billing.Api.InvoiceApi' is not a valid identifier."], Reported(result));
        Assert.Empty(output.Written);
    }

    [Fact]
    public void Reports_an_invalid_dependency_together_with_an_invalid_container_name()
    {
        // Given
        var source = new InMemoryContainerSource(
            [new ContainerAttribute("Billing", "Invoice.Api").DeclaredOn()],
            [new DependsOnAttribute("Payment.Gateway").DeclaredOn("Billing.Api.PaymentClient")]);
        var output = new InMemoryDeploymentUnitDslOutput();
        var handler = new GenerateContainerDslHandler(source, output);

        // When
        var result = handler.Handle();

        // Then
        Assert.Equal(
            [
                "PSTRUCT.0002: Container name 'Invoice.Api' on class 'Billing.Api.InvoiceApi' is not a valid identifier.",
                "PSTRUCT.0004: Dependency identifier 'Payment.Gateway' on class 'Billing.Api.PaymentClient' is not a valid identifier."
            ],
            Reported(result));
        Assert.Empty(output.Written);
    }
}
