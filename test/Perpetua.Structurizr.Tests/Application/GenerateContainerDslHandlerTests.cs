using Perpetua.Structurizr.Application;
using Perpetua.Structurizr.Domain;
using static Perpetua.Structurizr.Domain.ContainerDeclarationResult;

namespace Perpetua.Structurizr.Tests;

public class GenerateContainerDslHandlerTests
{
    private static string[] Reported(ContainerDeclarationResult result)
        => [.. Assert.IsType<Rejected>(result).Problems.Select(problem => $"{problem.Code}: {problem.Message}")];

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
        Assert.Equal("Website", context.Container.Name.Value);
    }

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
    public void Writes_nothing_when_a_dependency_identifier_is_not_a_valid_identifier()
    {
        // Given
        var source = new InMemoryContainerSource(
            [new ContainerAttribute("Billing", "InvoiceApi").DeclaredOn()],
            [new DependsOnAttribute("Payment.Gateway").DeclaredOn("Billing.Api.PaymentClient")]);
        var output = new InMemoryContextDeclarationDslOutput();
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
        var output = new InMemoryContextDeclarationDslOutput();
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
        var output = new InMemoryContextDeclarationDslOutput();
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
        var output = new InMemoryContextDeclarationDslOutput();
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
        var output = new InMemoryContextDeclarationDslOutput();
        var handler = new GenerateContainerDslHandler(source, output);

        // When
        var result = handler.Handle();

        // Then
        Assert.Equal(["PSTRUCT.0006: Dependency 'PaymentGateway' on class 'Billing.Api.InvoiceJob' is declared twice with the same context and description. Remove the duplicate."], Reported(result));
        Assert.Empty(output.Written);
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

    [Fact]
    public void Reports_an_invalid_dependency_together_with_an_invalid_container_name()
    {
        // Given
        var source = new InMemoryContainerSource(
            [new ContainerAttribute("Billing", "Invoice.Api").DeclaredOn()],
            [new DependsOnAttribute("Payment.Gateway").DeclaredOn("Billing.Api.PaymentClient")]);
        var output = new InMemoryContextDeclarationDslOutput();
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

    private static ContainerName Container(string value)
        => ((Result<ContainerName>.Success)ContainerName.Create(value)).Value;

    private static ContextName Context(string value)
        => ((Result<ContextName>.Success)ContextName.Create(value)).Value;
}
