using System.Text;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace Perpetua.Structurizr.Tests;

/// <summary>Validates workspace DSL against the real Structurizr tooling running in a container.</summary>
internal static class StructurizrCli
{
    /// <summary>Whether Structurizr accepts <paramref name="workspaceDsl"/> as valid DSL.</summary>
    public static async Task<bool> AcceptsAsync(string workspaceDsl)
    {
        var container = new ContainerBuilder("structurizr/structurizr")
            .WithResourceMapping(Encoding.UTF8.GetBytes(workspaceDsl), "/work/workspace.dsl")
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
