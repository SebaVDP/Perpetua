namespace Perpetua.Structurizr.Tests;

public class ProgramTests
{
    [Fact]
    public void Returns_success_when_the_assembly_directory_is_valid()
    {
        // Given
        var assemblyDirectory = Path.Combine(AppContext.BaseDirectory, "fixtures");
        var workingDirectory = Directory.CreateTempSubdirectory();

        try
        {
            // When
            var result = Program.Run([assemblyDirectory], workingDirectory, TextWriter.Null);

            // Then
            Assert.Equal(Program.CommandExitCode.Success, result);
            Assert.Equal(
                ["OtherSystem.dsl", "SampleSystem.dsl"],
                Directory.GetFiles(workingDirectory.FullName, "*.dsl")
                    .Select(Path.GetFileName)
                    .Order(StringComparer.Ordinal));
            Assert.Equal(
                """
                container "OtherContainer"
                container "OtherWorker"

                """.ReplaceLineEndings("\n"),
                File.ReadAllText(Path.Combine(workingDirectory.FullName, "OtherSystem.dsl")));
            Assert.Equal(
                """
                container "SampleContainer"
                container "SampleWorker"

                """.ReplaceLineEndings("\n"),
                File.ReadAllText(Path.Combine(workingDirectory.FullName, "SampleSystem.dsl")));
        }
        finally
        {
            workingDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void Reports_an_error_when_no_assembly_directory_is_supplied()
    {
        using var error = new StringWriter();

        var result = Program.Run([], new DirectoryInfo(Environment.CurrentDirectory), error);

        Assert.Equal(Program.CommandExitCode.InvalidArguments, result);
        Assert.Contains("directory", error.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Reports_an_error_when_the_assembly_directory_is_missing()
    {
        using var error = new StringWriter();

        var result = Program.Run(["does-not-exist"], new DirectoryInfo(Environment.CurrentDirectory), error);

        Assert.Equal(Program.CommandExitCode.InvalidArguments, result);
        Assert.Contains("directory", error.ToString(), StringComparison.OrdinalIgnoreCase);
    }
}
