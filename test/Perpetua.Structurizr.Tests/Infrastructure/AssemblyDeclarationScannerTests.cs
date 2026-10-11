using Perpetua.Structurizr.Infrastructure;

namespace Perpetua.Structurizr.Tests;

public class AssemblyDeclarationScannerTests
{
    [Fact]
    public void Finds_containers_declared_in_the_scanned_assemblies()
    {
        var directory = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "fixtures"));

        var containers = new AssemblyDeclarationScanner(directory).Containers();

        Assert.Contains(containers, container =>
            container.Context == "Billing" && container.Name == "InvoiceApi");
    }

    [Fact]
    public void Finds_the_class_that_declares_a_container()
    {
        var directory = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "fixtures"));

        var containers = new AssemblyDeclarationScanner(directory).Containers();

        Assert.Contains(containers, container =>
            container.Name == "InvoiceApi" && container.DeclaringClass.FullName.EndsWith(".InvoiceApi"));
    }

    [Fact]
    public void Ignores_files_that_are_not_assemblies()
    {
        var directory = Directory.CreateTempSubdirectory();
        File.WriteAllText(Path.Combine(directory.FullName, "notes.txt"), "not an assembly");

        try
        {
            var containers = new AssemblyDeclarationScanner(directory).Containers();

            Assert.Empty(containers);
        }
        finally
        {
            directory.Delete(recursive: true);
        }
    }

    [Fact]
    public void Finds_containers_in_assemblies_with_types_that_depend_on_unavailable_frameworks()
    {
        var assemblyDirectory = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "framework-dependent-fixtures"));

        var containers = new AssemblyDeclarationScanner(assemblyDirectory).Containers();

        Assert.Contains(containers, container =>
            container.Context == "Billing" && container.Name == "ReportingService");
    }
}
