using Perpetua.Structurizr.Domain;
using Perpetua.Structurizr.Infrastructure;

namespace Perpetua.Structurizr.Tests;

public class ContextDeclarationDslWriterStructurizrTests
{
    [Fact]
    public async Task Generated_container_declaration_is_accepted_inside_a_manually_authored_software_system()
    {
        // Given
        var workspaceDirectory = Directory.CreateTempSubdirectory();
        var writer = new ContextDeclarationDslWriter(workspaceDirectory);

        try
        {
            File.WriteAllText(
                Path.Combine(workspaceDirectory.FullName, "workspace.dsl"),
                """
                workspace {
                    !identifiers hierarchical
                    model {
                        SampleSystem = softwareSystem "SampleSystem" {
                            !include SampleSystem/containers
                        }
                    }
                }
                """);

            // When
            writer.Write(new ContextDeclaration(Context("SampleSystem"), Container("Website")));

            // Then
            Assert.True(await StructurizrCli.AcceptsAsync(workspaceDirectory));
        }
        finally
        {
            workspaceDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public async Task Containers_with_the_same_identifier_are_accepted_in_different_software_systems()
    {
        // Given
        var workspaceDirectory = Directory.CreateTempSubdirectory();
        var writer = new ContextDeclarationDslWriter(workspaceDirectory);

        try
        {
            File.WriteAllText(
                Path.Combine(workspaceDirectory.FullName, "workspace.dsl"),
                """
                workspace {
                    !identifiers hierarchical
                    model {
                        Billing = softwareSystem "Billing" {
                            !include Billing/containers
                        }
                        Accounts = softwareSystem "Accounts" {
                            !include Accounts/containers
                        }
                    }
                }
                """);

            // When
            writer.Write(new ContextDeclaration(Context("Billing"), Container("Website")));
            writer.Write(new ContextDeclaration(Context("Accounts"), Container("Website")));

            // Then
            Assert.True(await StructurizrCli.AcceptsAsync(workspaceDirectory));
        }
        finally
        {
            workspaceDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public async Task Relationships_are_accepted_when_included_after_the_containers_of_all_software_systems()
    {
        // Given
        var workspaceDirectory = Directory.CreateTempSubdirectory();
        var writer = new ContextDeclarationDslWriter(workspaceDirectory);

        try
        {
            File.WriteAllText(
                Path.Combine(workspaceDirectory.FullName, "workspace.dsl"),
                """
                workspace {
                    !identifiers hierarchical
                    model {
                        Billing = softwareSystem "Billing" {
                            !include Billing/containers
                        }
                        Accounts = softwareSystem "Accounts" {
                            !include Accounts/containers
                        }
                        !include Billing/relationships
                        !include Accounts/relationships
                    }
                }
                """);

            // When
            writer.Write(new ContextDeclaration(Context("Billing"), Container("Website")));
            writer.Write(new ContextDeclaration(Context("Accounts"), Container("Api")));

            // Then
            Assert.True(await StructurizrCli.AcceptsAsync(workspaceDirectory));
        }
        finally
        {
            workspaceDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public async Task Relationships_are_accepted_to_containers_with_the_same_identifier_in_different_software_systems()
    {
        // Given
        var workspaceDirectory = Directory.CreateTempSubdirectory();
        var writer = new ContextDeclarationDslWriter(workspaceDirectory);

        try
        {
            File.WriteAllText(
                Path.Combine(workspaceDirectory.FullName, "workspace.dsl"),
                """
                workspace {
                    !identifiers hierarchical
                    model {
                        A = softwareSystem "A" {
                            !include A/containers
                        }
                        B = softwareSystem "B" {
                            !include B/containers
                        }
                        C = softwareSystem "C" {
                            !include C/containers
                        }
                        !include C/relationships
                    }
                }
                """);

            // When
            writer.Write(new ContextDeclaration(Context("A"), Container("ABC")));
            writer.Write(new ContextDeclaration(Context("B"), Container("ABC")));
            writer.Write(new ContextDeclaration(Context("C"), Container("CBA"))
            {
                Dependencies =
                [
                    new DependencyDeclaration(Container("ABC"), Context("A"), null),
                    new DependencyDeclaration(Container("ABC"), Context("B"), null)
                ]
            });

            // Then
            Assert.True(await StructurizrCli.AcceptsAsync(workspaceDirectory));
        }
        finally
        {
            workspaceDirectory.Delete(recursive: true);
        }
    }

    private static ContextName Context(string value)
        => ((Result<ContextName>.Success)ContextName.Create(value)).Value;

    private static ContainerName Container(string value)
        => ((Result<ContainerName>.Success)ContainerName.Create(value)).Value;
}
