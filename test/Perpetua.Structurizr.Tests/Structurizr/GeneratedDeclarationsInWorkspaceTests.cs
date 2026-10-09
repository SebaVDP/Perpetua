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
}
