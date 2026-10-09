namespace Perpetua.Structurizr;

public static class Program
{
    public static int Main(string[] args)
        => (int)Run(args, Console.Out, Console.Error);

    internal static CommandExitCode Run(string[] args, TextWriter output, TextWriter error)
    {
        if (args.Length != 1 || !Directory.Exists(args[0]))
        {
            error.WriteLine("Provide a directory containing the assemblies to scan.");
            return CommandExitCode.InvalidArguments;
        }

        GenerateWorkspace(new DirectoryInfo(args[0]), output);
        return CommandExitCode.Success;
    }

    private static void GenerateWorkspace(DirectoryInfo assemblyDirectory, TextWriter output)
    {
        var containers = new ContainerScanner().Scan(assemblyDirectory);
        output.Write(new WorkspaceDslWriter().Write(containers));
    }

    internal enum CommandExitCode
    {
        Success = 0,
        InvalidArguments = 1
    }
}
