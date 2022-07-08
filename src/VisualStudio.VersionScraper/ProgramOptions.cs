namespace VisualStudio.VersionScraper;

using System.CommandLine;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Created at runtime.")]
internal class ProgramOptions
{
    public bool NoCache { get; set; }

    public string? OutputFilePath { get; set; }

    public static RootCommand Configure(Func<ProgramOptions, int> invokeAction)
    {
        Argument<string?> outputArgument = new(
            name: "output",
            description: "Json file output path.");

        Option<bool> noCacheOption = new(
            name: "--no-cache",
            description: "Skip the cache when making requests.");

        RootCommand rootCommand = new("Visual Studio Version Scraper")
        {
            outputArgument,
            noCacheOption
        };

        rootCommand.SetHandler(options =>
        {
            return Task.FromResult(invokeAction(options));
        }, new ProgramOptionsBinder(outputArgument, noCacheOption));

        return rootCommand;
    }
}
