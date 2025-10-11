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
        Argument<string?> outputArgument = new("output")
        {
            Description = "The output file path."
        };

        Option<OutputFormat> formatOption = new("--format", "-f")
        {
            Description = "The output file format."
        };

        Option<bool> noCacheOption = new("--no-cache")
        {
            Description = "Skip the cache when making requests."
        };

        RootCommand rootCommand = new("Visual Studio Version Scraper")
        {
            outputArgument,
            formatOption,
            noCacheOption,
        };

        rootCommand.SetAction(parseResult =>
        {
            ProgramOptions options = new()
            {
                OutputFilePath = parseResult.GetValue(outputArgument),
                Format = parseResult.GetValue(formatOption),
                NoCache = parseResult.GetValue(noCacheOption)
            };

            return invokeAction(options);
        });

        return rootCommand;
    }
}
