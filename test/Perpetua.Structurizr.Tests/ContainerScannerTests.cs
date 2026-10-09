namespace Perpetua.Structurizr.Tests;

public class ContainerScannerTests
{
    [Fact]
    public void Finds_containers_declared_in_the_scanned_assemblies()
    {
        var directory = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "fixtures"));

        var containers = new ContainerScanner().Scan(directory);

        Assert.Contains(containers, container =>
            container.Context == "SampleSystem" && container.Name == "SampleContainer");
    }

    [Fact]
    public void Ignores_files_that_are_not_assemblies()
    {
        var directory = Directory.CreateTempSubdirectory();
        File.WriteAllText(Path.Combine(directory.FullName, "notes.txt"), "not an assembly");

        try
        {
            var containers = new ContainerScanner().Scan(directory);

            Assert.Empty(containers);
        }
        finally
        {
            directory.Delete(recursive: true);
        }
    }
}
