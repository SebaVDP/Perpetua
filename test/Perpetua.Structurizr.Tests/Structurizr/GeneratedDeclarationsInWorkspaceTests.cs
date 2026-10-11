using Perpetua.Structurizr.Domain;
using Perpetua.Structurizr.Infrastructure;

namespace Perpetua.Structurizr.Tests;

public class GeneratedDeclarationsInWorkspaceTests
{
    [Fact]
    public async Task Generated_container_declaration_is_accepted_inside_a_manually_authored_software_system()
    {
        // Given
        var assemblyDirectory = Path.Combine(AppContext.BaseDirectory, "fixtures");
        var workspaceDirectory = Directory.CreateTempSubdirectory();

        try
        {
            Program.Run([assemblyDirectory, workspaceDirectory.FullName], workspaceDirectory, TextWriter.Null);
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
            var accepted = await StructurizrCli.AcceptsAsync(workspaceDirectory);

            // Then
            Assert.True(accepted);
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
            writer.Write(new ContextDeclaration(Context("Billing"), new ContainerDeclaration(Container("Website"))));
            writer.Write(new ContextDeclaration(Context("Accounts"), new ContainerDeclaration(Container("Website"))));
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
            var accepted = await StructurizrCli.AcceptsAsync(workspaceDirectory);

            // Then
            Assert.True(accepted);
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
            writer.Write(new ContextDeclaration(Context("Billing"), new ContainerDeclaration(Container("Website"))));
            writer.Write(new ContextDeclaration(Context("Accounts"), new ContainerDeclaration(Container("Api"))));
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
            var accepted = await StructurizrCli.AcceptsAsync(workspaceDirectory);

            // Then
            Assert.True(accepted);
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
            writer.Write(new ContextDeclaration(Context("A"), new ContainerDeclaration(Container("ABC"))));
            writer.Write(new ContextDeclaration(Context("B"), new ContainerDeclaration(Container("ABC"))));
            writer.Write(new ContextDeclaration(Context("C"), new ContainerDeclaration(Container("CBA")))
            {
                Dependencies =
                [
                    new DependencyDeclaration(Container("ABC"), Context("A"), null),
                    new DependencyDeclaration(Container("ABC"), Context("B"), null)
                ]
            });
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
            var accepted = await StructurizrCli.AcceptsAsync(workspaceDirectory);

            // Then
            Assert.True(accepted);
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
