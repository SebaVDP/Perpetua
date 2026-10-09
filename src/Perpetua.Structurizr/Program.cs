using Perpetua.Structurizr.Application;
using Perpetua.Structurizr.Infrastructure;

namespace Perpetua.Structurizr;

public static class Program
{
    public static int Main(string[] args)
        => (int)Run(args, new DirectoryInfo(Environment.CurrentDirectory), Console.Error);

    internal static CommandExitCode Run(string[] args, DirectoryInfo workingDirectory, TextWriter error)
    {
        var assemblyDirectory = args.Length >= 1 ? new DirectoryInfo(args[0]) : workingDirectory;
        if (args.Length > 2 || !assemblyDirectory.Exists)
        {
            error.WriteLine("Provide a directory containing the assemblies to scan.");
            return CommandExitCode.InvalidArguments;
        }

        var outputDirectory = args.Length == 2 ? new DirectoryInfo(args[1]) : workingDirectory;
        WriteContainerDslFiles(assemblyDirectory, outputDirectory);
        return CommandExitCode.Success;
    }

    private static void WriteContainerDslFiles(DirectoryInfo assemblyDirectory, DirectoryInfo outputDirectory)
    {
        var containerSource = new ContainerScanner(assemblyDirectory);
        var output = new ContainerDslWriter(outputDirectory);
        new GenerateContainerDslHandler(containerSource, output).Handle();
    }

    internal enum CommandExitCode
    {
        Success = 0,
        InvalidArguments = 1
    }
}
