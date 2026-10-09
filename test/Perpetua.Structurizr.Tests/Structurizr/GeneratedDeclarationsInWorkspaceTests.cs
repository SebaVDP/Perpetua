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
        var writer = new ContainerDslWriter(workspaceDirectory);
        Identifier.TryCreate("Website", out var website);

        try
        {
            writer.Write(new ContextDeclaration("Billing", new ContainerDeclaration(website!)));
            writer.Write(new ContextDeclaration("Accounts", new ContainerDeclaration(website!)));
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
}
