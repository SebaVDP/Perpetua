using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace Perpetua.Structurizr.Tests;

/// <summary>Validates workspace DSL against the real Structurizr tooling running in a container.</summary>
internal static class StructurizrCli
{
    /// <summary>Whether Structurizr accepts the <c>workspace.dsl</c> in <paramref name="workspaceDirectory"/>, including any files it includes.</summary>
    public static async Task<bool> AcceptsAsync(DirectoryInfo workspaceDirectory)
    {
        var container = new ContainerBuilder("structurizr/structurizr")
            .WithResourceMapping(workspaceDirectory, "/work")
            .WithCommand("validate", "-workspace", "/work/workspace.dsl")
            .Build();

        using var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(2));
        try
        {
            try
            {
                await container.StartAsync(timeout.Token);
            }
            catch (ContainerNotRunningException)
            {
                // The validate command runs once and exits, so a stopped container is expected.
            }

            return await container.GetExitCodeAsync(timeout.Token) == 0;
        }
        finally
        {
            await container.DisposeAsync();
        }
    }
}
