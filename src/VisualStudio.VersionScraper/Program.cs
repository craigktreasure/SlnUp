namespace VisualStudio.VersionScraper;

using CommandLine;
using SlnUp.Core;

internal static class Program
{
    public static int Main(string[] args)
    {
        Console.Title = "Visual Studio Version Scraper";

        return ProgramOptions.ParseOptions(args).MapResult(Run, _ => 1);
    }

    private static int Run(ProgramOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.OutputFilePath))
        {
            Console.WriteLine("Invalid file path.");
            return 1;
        }

        VisualStudioVersionDocScraper docScraper = new();

        IEnumerable<VisualStudioVersionDetail> versions = docScraper.ScrapeVisualStudioVersions()
            .Where(v => v.IsPreview is false);

        VisualStudioVersionDetail.SaveToJson(versions, options.OutputFilePath);

        return 0;
    }
}
