namespace Perpetua.Structurizr.Tests;

internal sealed class TemporaryDirectory : IDisposable
{
    public DirectoryInfo Info { get; } = Directory.CreateTempSubdirectory();

    public void Dispose() => Info.Delete(recursive: true);
}
