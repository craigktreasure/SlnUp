namespace SlnUp;

using CommandLine;
using System.IO.Abstractions;

internal static class Program
{
    public static int Main(string[] args)
    {
        Console.Title = "Visual Studio Solution Updater";

        return ProgramOptions.ParseOptions(args).MapResult(Run, _ => 1);
    }

    private static int Run(ProgramOptions options)
        => Run(new FileSystem(), options);

    private static int Run(IFileSystem fileSystem, ProgramOptions programOptions)
    {
        if (!programOptions.TryGetSlnUpOptions(fileSystem, out SlnUpOptions? options))
        {
            return 1;
        }

        return Run(fileSystem, options);
    }

    private static int Run(IFileSystem fileSystem, SlnUpOptions options)
    {
        return 0;
    }
}
