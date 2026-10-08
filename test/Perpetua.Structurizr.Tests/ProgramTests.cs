namespace Perpetua.Structurizr.Tests;

public class ProgramTests
{
    [Fact]
    public void Reports_an_error_when_the_assembly_directory_is_missing()
    {
        using var error = new StringWriter();

        var exitCode = Program.Run(["does-not-exist"], TextWriter.Null, error);

        Assert.Equal(1, exitCode);
        Assert.Contains("directory", error.ToString(), StringComparison.OrdinalIgnoreCase);
    }
}
