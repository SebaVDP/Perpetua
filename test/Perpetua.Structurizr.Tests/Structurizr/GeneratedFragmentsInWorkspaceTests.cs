namespace Perpetua.Structurizr.Tests;

public class GeneratedFragmentsInWorkspaceTests
{
    [Fact]
    public async Task Generated_fragment_is_accepted_inside_a_manually_authored_software_system()
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
                    model {
                        softwareSystem "SampleSystem" {
                            !include SampleSystem.dsl
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
