namespace Perpetua.Structurizr.Tests;

public class ProgramTests
{
    [Fact]
    public void Writes_the_files_to_the_output_directory_when_one_is_supplied()
    {
        // Given
        var assemblyDirectory = Path.Combine(AppContext.BaseDirectory, "fixtures");
        var workingDirectory = Directory.CreateTempSubdirectory();
        var outputDirectory = Directory.CreateTempSubdirectory();

        try
        {
            // When
            var result = Program.Run([assemblyDirectory, outputDirectory.FullName], workingDirectory, TextWriter.Null);

            // Then
            Assert.Equal(CommandExitCode.Success, result);
            Assert.Equal(
                """
                SampleContainer = container "SampleContainer"

                """.ReplaceLineEndings("\n"),
                File.ReadAllText(Path.Combine(outputDirectory.FullName, "SampleSystem", "containers", "SampleContainer.dsl")));
            Assert.Empty(Directory.GetFiles(workingDirectory.FullName));
        }
        finally
        {
            workingDirectory.Delete(recursive: true);
            outputDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void Uses_the_working_directory_for_input_and_output_when_no_directories_are_supplied()
    {
        // Given
        var workingDirectory = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "fixtures"));

        try
        {
            // When
            var result = Program.Run([], workingDirectory, TextWriter.Null);

            // Then
            Assert.Equal(CommandExitCode.Success, result);
            Assert.True(File.Exists(Path.Combine(
                workingDirectory.FullName, "SampleSystem", "containers", "SampleContainer.dsl")));
        }
        finally
        {
            Directory.Delete(Path.Combine(workingDirectory.FullName, "SampleSystem"), recursive: true);
        }
    }

    [Fact]
    public void Reports_an_error_when_more_than_one_container_is_found()
    {
        // Given
        var source = new InMemoryContainerSource(
            new ContainerAttribute("Billing", "Invoicing"),
            new ContainerAttribute("Accounts", "Ledger"));
        var output = new InMemoryContainerDslOutput();
        using var error = new StringWriter();

        // When
        var result = Program.Generate(source, output, error);

        // Then
        Assert.Equal(CommandExitCode.OnlyOneContainerAllowed, result);
        Assert.Contains("one container", error.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Reports_an_error_when_the_container_name_is_not_a_valid_identifier()
    {
        // Given
        var source = new InMemoryContainerSource(new ContainerAttribute("ExampleSystem", "a.b"));
        var output = new InMemoryContainerDslOutput();
        using var error = new StringWriter();

        // When
        var result = Program.Generate(source, output, error);

        // Then
        Assert.Equal(CommandExitCode.InvalidContainerName, result);
        Assert.Contains("identifier", error.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Reports_an_error_when_the_context_is_not_a_valid_identifier()
    {
        // Given
        var source = new InMemoryContainerSource(new ContainerAttribute("Example.System", "Website"));
        var output = new InMemoryContainerDslOutput();
        using var error = new StringWriter();

        // When
        var result = Program.Generate(source, output, error);

        // Then
        Assert.Equal(CommandExitCode.InvalidContextName, result);
        Assert.Contains("context", error.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Overwrites_the_declaration_of_an_earlier_run_with_the_same_context_and_identifier()
    {
        // Given
        var assemblyDirectory = Path.Combine(AppContext.BaseDirectory, "fixtures");
        var workingDirectory = Directory.CreateTempSubdirectory();
        var outputDirectory = Directory.CreateTempSubdirectory();
        var earlierRun = Path.Combine(outputDirectory.FullName, "SampleSystem", "containers", "SampleContainer.dsl");
        Directory.CreateDirectory(Path.GetDirectoryName(earlierRun)!);
        File.WriteAllText(earlierRun, "stale");

        try
        {
            // When
            var result = Program.Run([assemblyDirectory, outputDirectory.FullName], workingDirectory, TextWriter.Null);

            // Then
            Assert.Equal(CommandExitCode.Success, result);
            Assert.Equal(
                "SampleContainer = container \"SampleContainer\"\n",
                File.ReadAllText(earlierRun));
        }
        finally
        {
            workingDirectory.Delete(recursive: true);
            outputDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void Reports_an_error_when_the_assembly_directory_is_missing()
    {
        using var error = new StringWriter();

        var result = Program.Run(["does-not-exist"], new DirectoryInfo(Environment.CurrentDirectory), error);

        Assert.Equal(CommandExitCode.InvalidArguments, result);
        Assert.Contains("directory", error.ToString(), StringComparison.OrdinalIgnoreCase);
    }
}
