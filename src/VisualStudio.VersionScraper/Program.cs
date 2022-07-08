namespace VisualStudio.VersionScraper;

using SlnUp.Core;
using System.CommandLine;
using System.IO.Abstractions;

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
            .Where(v => v.IsPreview is false);

        IFileSystem fileSystem = new FileSystem();
        VisualStudioVersion.ToJsonFile(fileSystem, versions, options.OutputFilePath);

        return 0;
    }
}
