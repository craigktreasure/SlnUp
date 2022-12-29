namespace VisualStudio.VersionScraper;

using System.CommandLine;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Created at runtime.")]
internal sealed class ProgramOptions
{
    public OutputFormat Format { get; set; }

    public bool NoCache { get; set; }

    public string? OutputFilePath { get; set; }

    public static RootCommand Configure(Func<ProgramOptions, int> invokeAction)
    {
        Argument<string?> outputArgument = new(
            name: "output",
            description: "The output file path.");

        Option<OutputFormat> formatOption = new(
            name: "--format",
            description: "The output file format.");
        formatOption.AddAlias("-f");

        Option<bool> noCacheOption = new(
            name: "--no-cache",
            description: "Skip the cache when making requests.");

        RootCommand rootCommand = new("Visual Studio Version Scraper")
        {
            outputArgument,
            formatOption,
            noCacheOption,
        };

        rootCommand.SetHandler(options =>
        {
            return Task.FromResult(invokeAction(options));
        }, new ProgramOptionsBinder(outputArgument, formatOption, noCacheOption));

        return rootCommand;
    }
}
