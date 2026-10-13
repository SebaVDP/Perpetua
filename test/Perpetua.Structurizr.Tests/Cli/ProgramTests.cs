namespace Perpetua.Structurizr.Tests;

public class ProgramTests
{
    [Fact]
    public void Writes_the_files_to_the_output_directory_when_one_is_supplied()
    {
        // Given
        var assemblyDirectory = Path.Combine(AppContext.BaseDirectory, "fixtures");
        using var workingDirectory = new TemporaryDirectory();
        using var outputDirectory = new TemporaryDirectory();

        // When
        var result = Program.Run([assemblyDirectory, outputDirectory.Info.FullName], workingDirectory.Info, TextWriter.Null);

        // Then
        Assert.Equal(CommandExitCode.Success, result);
        Assert.Equal(
            """
            InvoiceApi = container "InvoiceApi"
            InvoiceApi_OrderPlaced = container "InvoiceApi_OrderPlaced" "" "" "Event"

            """.ReplaceLineEndings("\n"),
            File.ReadAllText(Path.Combine(outputDirectory.Info.FullName, "Billing", "containers", "InvoiceApi.dsl")));
        Assert.Empty(Directory.GetFiles(workingDirectory.Info.FullName));
        var relationships = File.ReadAllLines(
            Path.Combine(outputDirectory.Info.FullName, "Billing", "relationships", "InvoiceApi.dsl"));
        Assert.Equal(
            new[]
            {
                "Billing.InvoiceApi -> Accounting.LedgerApi",
                "Billing.InvoiceApi -> Accounting.LedgerApi_PaymentSettled",
                "Billing.InvoiceApi -> Billing.InvoiceApi_OrderPlaced",
                "Billing.InvoiceApi -> Billing.PaymentGateway",
                "Billing.InvoiceApi -> Compliance.AuditTrail \"Records changes\""
            },
            relationships.Where(line => line.Length > 0).Order().ToArray());
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
                workingDirectory.FullName, "Billing", "containers", "InvoiceApi.dsl")));
        }
        finally
        {
            Directory.Delete(Path.Combine(workingDirectory.FullName, "Billing"), recursive: true);
        }
    }

    [Fact]
    public void Prints_every_problem_as_a_code_and_a_message()
    {
        // Given
        var source = new InMemoryContainerSource(
            [new ContainerAttribute("Invoice.Api", "Billing").DeclaredOn()],
            [new DependsOnAttribute("Payment.Gateway").DeclaredOn("Billing.Api.PaymentClient")]);
        var output = new InMemoryDeploymentUnitDslOutput();
        using var error = new StringWriter();

        // When
        var result = Program.Generate(source, output, error);

        // Then
        Assert.Equal(CommandExitCode.Failure, result);
        Assert.Equal(
            """
            PSTRUCT.0002: Container name 'Invoice.Api' on class 'Billing.Api.InvoiceApi' is not a valid identifier.
            PSTRUCT.0004: Dependency name 'Payment.Gateway' on class 'Billing.Api.PaymentClient' is not a valid identifier.
            """.ReplaceLineEndings(),
            error.ToString().Trim());
    }

    [Fact]
    public void Overwrites_the_declaration_of_an_earlier_run_with_the_same_context_and_identifier()
    {
        // Given
        var assemblyDirectory = Path.Combine(AppContext.BaseDirectory, "fixtures");
        using var workingDirectory = new TemporaryDirectory();
        using var outputDirectory = new TemporaryDirectory();
        var earlierRun = Path.Combine(outputDirectory.Info.FullName, "Billing", "containers", "InvoiceApi.dsl");
        Directory.CreateDirectory(Path.GetDirectoryName(earlierRun)!);
        File.WriteAllText(earlierRun, "stale");

        // When
        var result = Program.Run([assemblyDirectory, outputDirectory.Info.FullName], workingDirectory.Info, TextWriter.Null);

        // Then
        Assert.Equal(CommandExitCode.Success, result);
        Assert.StartsWith("InvoiceApi = container \"InvoiceApi\"\n", File.ReadAllText(earlierRun));
    }

    [Fact]
    public void Reports_an_error_when_the_assembly_directory_is_missing()
    {
        using var error = new StringWriter();

        var result = Program.Run(["does-not-exist"], new DirectoryInfo(Environment.CurrentDirectory), error);

        Assert.Equal(CommandExitCode.Failure, result);
        Assert.Contains("directory", error.ToString(), StringComparison.OrdinalIgnoreCase);
    }
}
