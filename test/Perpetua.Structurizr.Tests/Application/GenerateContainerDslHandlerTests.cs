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
        var generated =         Assert.IsType<DeclarationGenerated>(result);
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
            [new ContainerAttribute("Billing", "CBA")],
            new DependsOnAttribute("ABC"));
        var output = new InMemoryContextDeclarationDslOutput();
        var handler = new GenerateContainerDslHandler(source, output);

        // When
        var result = handler.Handle();

        // Then
        var generated = Assert.IsType<DeclarationGenerated>(result);
        Assert.Same(generated.Declaration, Assert.Single(output.Written));
        Assert.Equal(new DependencyDeclaration(Container("ABC"), Context("Billing"), null), Assert.Single(generated.Declaration.Dependencies));
    }

    [Fact]
    public void Writes_a_relationship_to_the_specified_context()
    {
        // Given
        var source = new InMemoryContainerSource(
            [new ContainerAttribute("Billing", "CBA")],
            new DependsOnAttribute("ABC", "Accounts"));
        var output = new InMemoryContextDeclarationDslOutput();
        var handler = new GenerateContainerDslHandler(source, output);

        // When
        var result = handler.Handle();

        // Then
        var generated = Assert.IsType<DeclarationGenerated>(result);
        Assert.Same(generated.Declaration, Assert.Single(output.Written));
        Assert.Equal(new DependencyDeclaration(Container("ABC"), Context("Accounts"), null), Assert.Single(generated.Declaration.Dependencies));
    }

    [Fact]
    public void Writes_nothing_when_a_dependency_identifier_is_not_a_valid_identifier()
    {
        // Given
        var source = new InMemoryContainerSource(
            [new ContainerAttribute("Billing", "CBA")],
            new DependsOnAttribute("A.B"));
        var output = new InMemoryContextDeclarationDslOutput();
        var handler = new GenerateContainerDslHandler(source, output);

        // When
        var result = handler.Handle();

        // Then
        Assert.Equal([new Problem.InvalidDependencyIdentifier()], Assert.IsType<Rejected>(result).Problems);
        Assert.Empty(output.Written);
    }

    [Fact]
    public void Writes_nothing_when_a_dependency_context_is_not_a_valid_identifier()
    {
        // Given
        var source = new InMemoryContainerSource(
            [new ContainerAttribute("Billing", "CBA")],
            new DependsOnAttribute("ABC", "A.B"));
        var output = new InMemoryContextDeclarationDslOutput();
        var handler = new GenerateContainerDslHandler(source, output);

        // When
        var result = handler.Handle();

        // Then
        Assert.Equal([new Problem.InvalidDependencyContext()], Assert.IsType<Rejected>(result).Problems);
        Assert.Empty(output.Written);
    }

    [Fact]
    public void Writes_nothing_when_the_same_dependency_is_declared_twice()
    {
        // Given
        var source = new InMemoryContainerSource(
            [new ContainerAttribute("Billing", "CBA")],
            new DependsOnAttribute("ABC", "Accounts", "Pays invoices"),
            new DependsOnAttribute("ABC", "Accounts", "Pays invoices"));
        var output = new InMemoryContextDeclarationDslOutput();
        var handler = new GenerateContainerDslHandler(source, output);

        // When
        var result = handler.Handle();

        // Then
        Assert.Equal([new Problem.DuplicateDependency()], Assert.IsType<Rejected>(result).Problems);
        Assert.Empty(output.Written);
    }

    [Fact]
    public void Writes_nothing_when_a_dependency_is_declared_twice_with_and_without_the_container_context()
    {
        // Given
        var source = new InMemoryContainerSource(
            [new ContainerAttribute("Billing", "CBA")],
            new DependsOnAttribute("ABC"),
            new DependsOnAttribute("ABC", "Billing"));
        var output = new InMemoryContextDeclarationDslOutput();
        var handler = new GenerateContainerDslHandler(source, output);

        // When
        var result = handler.Handle();

        // Then
        Assert.Equal([new Problem.DuplicateDependency()], Assert.IsType<Rejected>(result).Problems);
        Assert.Empty(output.Written);
    }

    [Fact]
    public void Writes_both_relationships_when_the_same_container_is_a_dependency_in_two_contexts()
    {
        // Given
        var source = new InMemoryContainerSource(
            [new ContainerAttribute("Billing", "CBA")],
            new DependsOnAttribute("ABC", "A"),
            new DependsOnAttribute("ABC", "B"));
        var output = new InMemoryContextDeclarationDslOutput();
        var handler = new GenerateContainerDslHandler(source, output);

        // When
        var result = handler.Handle();

        // Then
        var generated = Assert.IsType<DeclarationGenerated>(result);
        Assert.Equal(
            [
                new DependencyDeclaration(Container("ABC"), Context("A"), null),
                new DependencyDeclaration(Container("ABC"), Context("B"), null)
            ],
            generated.Declaration.Dependencies);
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
        var output = new InMemoryContextDeclarationDslOutput();
        var handler = new GenerateContainerDslHandler(source, output);

        // When
        var result = handler.Handle();

        // Then
        Assert.Equal([new Problem.InvalidContainerName()], Assert.IsType<Rejected>(result).Problems);
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
        Assert.Equal([new Problem.InvalidContextName()], Assert.IsType<Rejected>(result).Problems);
        Assert.Empty(output.Written);
    }

    [Fact]
    public void Writes_nothing_when_more_than_one_container_is_found()
    {
        // Given
        var source = new InMemoryContainerSource(
            new ContainerAttribute("Billing", "Invoicing"),
            new ContainerAttribute("Accounts", "Ledger"));
        var output = new InMemoryContextDeclarationDslOutput();
        var handler = new GenerateContainerDslHandler(source, output);

        // When
        var result = handler.Handle();

        // Then
        Assert.Equal([new Problem.OnlyOneContainerAllowed()], Assert.IsType<Rejected>(result).Problems);
        Assert.Empty(output.Written);
    }

    private static ContainerName Container(string value)
        => ((Result<ContainerName>.Success)ContainerName.Create(value)).Value;

    private static ContextName Context(string value)
        => ((Result<ContextName>.Success)ContextName.Create(value)).Value;
}
