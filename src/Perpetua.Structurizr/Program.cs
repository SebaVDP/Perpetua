namespace Perpetua.Structurizr;

public static class Program
{
    public static int Main(string[] args)
        => (int)Run(args, new DirectoryInfo(Environment.CurrentDirectory), Console.Error);

    internal static CommandExitCode Run(string[] args, DirectoryInfo workingDirectory, TextWriter error)
    {
        if (args.Length != 1 || !Directory.Exists(args[0]))
        {
            error.WriteLine("Provide a directory containing the assemblies to scan.");
            return CommandExitCode.InvalidArguments;
        }

        WriteContainerDslFiles(new DirectoryInfo(args[0]), workingDirectory);
        return CommandExitCode.Success;
    }

    private static void WriteContainerDslFiles(DirectoryInfo assemblyDirectory, DirectoryInfo workingDirectory)
    {
        var containers = new ContainerScanner().Scan(assemblyDirectory);
        var fragments = new ContainerDslFragmentsBuilder().Build(containers);
        new ContainerDslWriter().Write(fragments, workingDirectory);
    }

    internal enum CommandExitCode
    {
        Success = 0,
        InvalidArguments = 1
    }
}
