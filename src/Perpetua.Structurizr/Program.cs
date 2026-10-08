namespace Perpetua.Structurizr;

/// <summary>Runs the Structurizr workspace generator tool.</summary>
public static class Program
{
    /// <summary>Scans assemblies in the supplied directory and writes workspace DSL.</summary>
    public static int Main(string[] args)
        => Run(args, Console.Out, Console.Error);

    internal static int Run(string[] args, TextWriter output, TextWriter error)
    {
        if (args.Length != 1 || !Directory.Exists(args[0]))
        {
            error.WriteLine("Provide a directory containing the assemblies to scan.");
            return 1;
        }

        var containers = new ContainerScanner().Scan(args[0]);
        output.Write(new WorkspaceDslWriter().Write(containers));
        return 0;
    }
}
