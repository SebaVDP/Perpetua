using Perpetua.Structurizr.Domain;
using static Perpetua.Structurizr.Domain.ContainerDeclarationResult;

namespace Perpetua.Structurizr.Tests;

internal static class HandlerTestSupport
{
    public static string[] Reported(ContainerDeclarationResult result)
        => [.. Assert.IsType<Rejected>(result).Problems.Select(problem => $"{problem.Code}: {problem.Message}")];

    public static ContainerName Container(string value)
        => ((Result<ContainerName>.Success)ContainerName.Create(value)).Value;

    public static ContextName Context(string value)
        => ((Result<ContextName>.Success)ContextName.Create(value)).Value;
}
