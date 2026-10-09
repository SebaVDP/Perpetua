namespace Perpetua.Structurizr.Tests;

public class ProgramTests
{
    [Fact]
    public void Returns_success_when_the_assembly_directory_is_valid()
    {
        var assemblyDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var result = Program.Run([assemblyDirectory.FullName], TextWriter.Null, TextWriter.Null);

            Assert.Equal(Program.CommandExitCode.Success, result);
        }
        finally
        {
            assemblyDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void Reports_an_error_when_no_assembly_directory_is_supplied()
    {
        using var error = new StringWriter();

        var result = Program.Run([], TextWriter.Null, error);

        Assert.Equal(Program.CommandExitCode.InvalidArguments, result);
        Assert.Contains("directory", error.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Reports_an_error_when_the_assembly_directory_is_missing()
    {
        using var error = new StringWriter();

        var result = Program.Run(["does-not-exist"], TextWriter.Null, error);

        Assert.Equal(Program.CommandExitCode.InvalidArguments, result);
        Assert.Contains("directory", error.ToString(), StringComparison.OrdinalIgnoreCase);
    }
}
