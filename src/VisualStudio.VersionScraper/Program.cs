namespace VisualStudio.VersionScraper;

using System.CommandLine;
using System.IO.Abstractions;

using SlnUp.Core;
using SlnUp.Json;

using VisualStudio.VersionScraper.Writers.CSharp;

internal static class Program
{
    public static int Main(string[] args)
    {
        Console.Title = "Visual Studio Version Scraper";

        return ProgramOptions.Configure(Run).Invoke(args);
    }

    private static int Run(ProgramOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.OutputFilePath))
        {
            Console.WriteLine("Invalid file path.");
            return 1;
        }

        VisualStudioVersionDocScraper docScraper = new(useCache: options.NoCache is false);

        IEnumerable<VisualStudioVersion> versions = docScraper.ScrapeVisualStudioVersions()
            .Where(v => v.IsPreview is false)
            .OrderByDescending(x => x.BuildVersion);

        IFileSystem fileSystem = new FileSystem();

        switch (options.Format)
        {
            case OutputFormat.Json:
                VisualStudioVersionJsonHelper.ToJsonFile(fileSystem, versions, options.OutputFilePath);
                break;

            case OutputFormat.CSharp:
                CSharpVersionWriter csharpWriter = new(fileSystem);
                csharpWriter.WriteClassToFile(versions, options.OutputFilePath);
                break;

            default:
                throw new NotSupportedException($"The format is not supported: '{options.Format}'.");
        }

        using (ConsoleHelpers.WithForegroundColor(ConsoleColor.Green))
        {
            Console.WriteLine($"Done writing {options.Format} to '{options.OutputFilePath}'.");
        }

        return 0;
    }
}
