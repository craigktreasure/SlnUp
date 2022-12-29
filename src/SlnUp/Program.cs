namespace SlnUp;

using System.CommandLine;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;

using SlnUp.CLI;
using SlnUp.Core;

internal static class Program
{
    public static int Main(string[] args)
    {
        Console.Title = ThisAssembly.AssemblyTitle;

        return ProgramOptions.Configure(Run).Invoke(args);
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

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "By design.")]
    private static int Run(IFileSystem fileSystem, SlnUpOptions options)
    {
        try
        {
            SolutionFile solutionFile = new(fileSystem, options.SolutionFilePath);
            solutionFile.UpdateFileHeader(options.BuildVersion);
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
