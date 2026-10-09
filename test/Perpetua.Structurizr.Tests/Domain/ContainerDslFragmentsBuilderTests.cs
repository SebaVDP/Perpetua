using Perpetua.Structurizr.Domain;

namespace Perpetua.Structurizr.Tests;

public class ContainerDslFragmentsBuilderTests
{
    [Fact]
    public void Builds_the_context_fragment_of_the_container()
    {
        // Given
        var containers = new[]
        {
            new ContainerAttribute("ExampleSystem", "Website"),
        };

        // When
        var result = new ContainerDslFragmentsBuilder().Build(containers);

        // Then
        var context = Assert.IsType<ContextDslFragment>(result);
        Assert.Equal("ExampleSystem", context.ContextName);
        Assert.Equal(new ContainerDslFragment("Website"), Assert.Single(context.Containers));
    }

    [Fact]
    public void Reports_that_no_containers_are_found_when_the_deployment_unit_declares_none()
    {
        // When
        var result = new ContainerDslFragmentsBuilder().Build([]);

        // Then
        Assert.IsType<NoContainersFound>(result);
    }

    [Fact]
    public void Reports_an_error_when_more_than_one_container_is_found()
    {
        // Given
        var containers = new[]
        {
            new ContainerAttribute("Billing", "Invoicing"),
            new ContainerAttribute("Accounts", "Ledger"),
        };

        // When
        var result = new ContainerDslFragmentsBuilder().Build(containers);

        // Then
        Assert.IsType<OnlyOneContainerAllowed>(result);
    }
}
