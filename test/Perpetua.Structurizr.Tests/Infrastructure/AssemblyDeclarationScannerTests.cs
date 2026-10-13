using Perpetua.Structurizr.Infrastructure;

namespace Perpetua.Structurizr.Tests;

public class AssemblyDeclarationScannerTests
{
    private static AssemblyDeclarationScanner FrameworkDependentScanner()
        => new(new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "framework-dependent-fixtures")));

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
        var containers = FrameworkDependentScanner().Containers();

        Assert.Contains(containers, container =>
            container.Context == "Billing" && container.Name == "ReportingService");
    }

    [Fact]
    public void Finds_events_declared_on_types_that_depend_on_unavailable_frameworks()
    {
        var events = FrameworkDependentScanner().Events();

        Assert.Contains(events, declared => declared.Name == "ReportGenerated");
    }

    [Fact]
    public void Finds_dependencies_declared_on_types_that_depend_on_unavailable_frameworks()
    {
        var dependencies = FrameworkDependentScanner().Dependencies();

        Assert.Contains(dependencies, dependency => dependency.Identifier == "Catalog");
    }

    [Fact]
    public void Finds_consumptions_declared_on_types_that_depend_on_unavailable_frameworks()
    {
        var consumptions = FrameworkDependentScanner().Consumptions();

        Assert.Contains(consumptions, consumption => consumption.EventName == "BookBorrowed");
    }

    [Fact]
    public void Names_the_declaring_class_of_a_nested_type_with_its_outer_type()
    {
        var containers = FrameworkDependentScanner().Containers();

        Assert.Contains(containers, container =>
            container.Name == "ReportingExporter" && container.DeclaringClass.FullName.EndsWith(".ReportingModule+Exporter"));
    }

    [Fact]
    public void Ignores_attributes_defined_in_the_scanned_assembly_itself()
    {
        var containers = FrameworkDependentScanner().Containers();

        Assert.Contains(containers, container => container.Name == "MarkedService");
    }

    [Fact]
    public void Names_the_declaring_class_of_a_type_without_namespace_by_its_name_alone()
    {
        var containers = FrameworkDependentScanner().Containers();

        Assert.Contains(containers, container =>
            container.Name == "GlobalService" && container.DeclaringClass.FullName == "GlobalService");
    }
}
