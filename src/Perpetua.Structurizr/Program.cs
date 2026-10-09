using Perpetua.Structurizr.Application;
using Perpetua.Structurizr.Domain;
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
        return Generate(new ContainerScanner(assemblyDirectory), new ContainerDslWriter(outputDirectory), error);
    }

    internal static CommandExitCode Generate(IContainerSource source, IContainerDslOutput output, TextWriter error)
    {
        var result = new GenerateContainerDslHandler(source, output).Handle();
        if (result is OnlyOneContainerAllowed)
        {
            error.WriteLine("A deployment unit may declare only one container.");
            return CommandExitCode.OnlyOneContainerAllowed;
        }

        return CommandExitCode.Success;
    }

    internal enum CommandExitCode
    {
        Success = 0,
        InvalidArguments = 1,
        OnlyOneContainerAllowed = 2
    }
}
