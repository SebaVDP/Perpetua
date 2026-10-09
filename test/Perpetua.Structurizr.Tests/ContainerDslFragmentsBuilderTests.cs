namespace Perpetua.Structurizr.Tests;

public class ContainerDslFragmentsBuilderTests
{
    [Fact]
    public void Groups_container_fragments_by_context()
    {
        // Given
        var containers = new[]
        {
            new ContainerAttribute("ExampleSystem", "Website"),
        };

        // When
        var result = new ContainerDslFragmentsBuilder().Build(containers);

        // Then
        var fragments = Assert.IsType<ContainerDslFragments>(result);
        var context = Assert.Single(fragments.Contexts);
        Assert.Equal("ExampleSystem", context.ContextName);
        Assert.Equal(new ContainerDslFragment("Website"), Assert.Single(context.Containers));
    }

    [Fact]
    public void Orders_context_fragments_and_containers_alphabetically()
    {
        // Given
        var containers = new[]
        {
            new ContainerAttribute("Billing", "Worker"),
            new ContainerAttribute("Accounts", "Website"),
            new ContainerAttribute("Billing", "Api"),
            new ContainerAttribute("Accounts", "Database"),
        };

        // When
        var result = new ContainerDslFragmentsBuilder().Build(containers);

        // Then
        var fragments = Assert.IsType<ContainerDslFragments>(result);
        Assert.Equal(2, fragments.Contexts.Count);
        Assert.Equal("Accounts", fragments.Contexts[0].ContextName);
        Assert.Equal(
            [new ContainerDslFragment("Database"), new ContainerDslFragment("Website")],
            fragments.Contexts[0].Containers);
        Assert.Equal("Billing", fragments.Contexts[1].ContextName);
        Assert.Equal(
            [new ContainerDslFragment("Api"), new ContainerDslFragment("Worker")],
            fragments.Contexts[1].Containers);
    }
}
