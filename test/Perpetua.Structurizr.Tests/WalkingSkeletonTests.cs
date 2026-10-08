namespace Perpetua.Structurizr.Tests;

public class WalkingSkeletonTests
{
    [Fact]
    public async Task Generated_workspace_is_accepted_by_structurizr()
    {
        var workspace = GenerateWorkspace();

        var accepted = await StructurizrCli.AcceptsAsync(workspace);

        Assert.True(accepted);
    }

    private static string GenerateWorkspace()
    {
        var directory = Directory.CreateTempSubdirectory();
        using var output = new StringWriter();
        try
        {
            Program.Run([directory.FullName], output, TextWriter.Null);
            return output.ToString();
        }
        finally
        {
            directory.Delete(recursive: true);
        }
    }
}
