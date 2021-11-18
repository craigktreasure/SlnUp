namespace VisualStudio.VersionScraper;

using CommandLine;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Created at runtime.")]
internal class ProgramOptions
{
    [Option("no-cache", HelpText = "Skip the cache when making requests.")]
    public bool NoCache { get; set; }

    [Value(0, MetaName = "output", Required = true, HelpText = "Json file output path.")]
    public string? OutputFilePath { get; set; }

    public static ParserResult<ProgramOptions> ParseOptions(string[] args)
        => Parser.Default.ParseArguments<ProgramOptions>(args);
}
