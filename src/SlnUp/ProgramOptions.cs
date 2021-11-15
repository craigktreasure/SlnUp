namespace SlnUp;

using CommandLine;

internal class ProgramOptions
{
    [Option("build-version", HelpText = "Uses version information as specified with this build version number.")]
    public Version? BuildVersion { get; set; }

    [Option('p', "path", HelpText = "The path to the solution path.")]
    public string? SolutionPath { get; set; }

    [Value(0, Required = true, MetaName = "version", HelpText = "The Visual Studio version to update the solution file with.")]
    public string? Version { get; set; }

    public static ParserResult<ProgramOptions> ParseOptions(string[] args)
        => ParseOptions(args, Parser.Default);

    public static ParserResult<ProgramOptions> ParseOptions(string[] args, Parser parser)
        => parser.ParseArguments<ProgramOptions>(args);
}
