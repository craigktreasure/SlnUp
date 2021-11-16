namespace SlnUp;

using CommandLine;
using SlnUp.Core;
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

    internal static int Run(IFileSystem fileSystem, ProgramOptions programOptions)
    {
        if (!programOptions.TryGetSlnUpOptions(fileSystem, out SlnUpOptions? options))
        {
            return 1;
        }

        return Run(fileSystem, options);
    }

    internal static int Run(IFileSystem fileSystem, SlnUpOptions options)
    {
        try
        {
            SolutionFile solutionFile = new(fileSystem, options.SolutionFilePath);
            solutionFile.UpdateFileHeader(new SolutionFileHeader(options.BuildVersion));
        }
        catch (Exception ex)
        {
            using IDisposable _ = ConsoleHelpers.WithError();
            Console.WriteLine(ex.Message);
            return 1;
        }

        return 0;
    }
}
