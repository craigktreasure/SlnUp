namespace SlnUp;

using CommandLine;
using SlnUp.Core;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;

internal class ProgramOptions
{
    [Option("build-version", HelpText = "Uses version information as specified with this build version number.")]
    public Version? BuildVersion { get; set; }

    [Option('p', "path", HelpText = "The path to the solution path.")]
    public string? SolutionPath { get; set; }

    [Value(0, Required = true, MetaName = "version", HelpText = "The Visual Studio version to update the solution file"
        + "with. Should be either a 2 or 3-part version number or a product year.")]
    public string? Version { get; set; }

    /// <summary>
    /// Parses the options.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns><see cref="ParserResult{ProgramOptions}"/>.</returns>
    public static ParserResult<ProgramOptions> ParseOptions(string[] args)
        => ParseOptions(args, Parser.Default);

    /// <summary>
    /// Parses the options.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <param name="parser">The parser.</param>
    /// <returns><see cref="ParserResult{ProgramOptions}"/>.</returns>
    public static ParserResult<ProgramOptions> ParseOptions(string[] args, Parser parser)
        => parser.ParseArguments<ProgramOptions>(args);

    /// <summary>
    /// Tries to get <see cref="SlnUpOptions"/>.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="options">The options.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryGetSlnUpOptions(IFileSystem fileSystem, [NotNullWhen(true)] out SlnUpOptions? options)
    {
        options = null;

        if (!TryResolveSolutionFilePath(fileSystem, this.SolutionPath, out string? solutionFilePath))
        {
            return false;
        }

        if (!fileSystem.File.Exists(solutionFilePath))
        {
            using IDisposable _ = ConsoleHelpers.WithError();
            Console.WriteLine($"The solution file could not be found: '{solutionFilePath}'.");
            return false;
        }

        if (this.BuildVersion is not null)
        {
            // Use version information as provided.
            if (!this.BuildVersion.Is4PartVersion())
            {
                using IDisposable _ = ConsoleHelpers.WithError();
                Console.WriteLine("The build version must be a full 4-part version number.");
                return false;
            }

            if (!VersionManager.TryParseVisualStudioVersion(this.Version, out Version? version))
            {
                using IDisposable _ = ConsoleHelpers.WithError();
                Console.WriteLine("The version specified was not a valid 2 or 3-part version number.");
                return false;
            }

            options = new SlnUpOptions(solutionFilePath, version, this.BuildVersion);
        }
        else
        {
            // Try to lookup the version specified.
            VersionManager versionManager = VersionManager.LoadFromEmbeddedResource();

            VisualStudioVersion? version = versionManager.FromVersionParameter(this.Version);

            if (version is null)
            {
                using IDisposable _ = ConsoleHelpers.WithError();
                Console.WriteLine($"The version specified could not be resolved to a supported Visual Studio product version: '{this.Version}'.");
                return false;
            }

            options = new SlnUpOptions(solutionFilePath, version.Version, version.BuildVersion);
        }

        return options is not null;
    }

    private static bool TryResolveSolutionFilePath(IFileSystem fileSystem, string? input, [NotNullWhen(true)] out string? solutionFilePath)
    {
        solutionFilePath = null;
        string currentDirectory = fileSystem.Directory.GetCurrentDirectory();

        if (input is null)
        {
            IReadOnlyList<string> solutionFiles = fileSystem.Directory.EnumerateFiles(
                currentDirectory,
                "*.sln",
                SearchOption.TopDirectoryOnly)
                .ToArray();

            if (solutionFiles.Count is 0)
            {
                using IDisposable _ = ConsoleHelpers.WithError();

                Console.WriteLine("No solution (.sln) files could be found in the current directory.");

                return false;
            }

            if (solutionFiles.Count > 1)
            {
                using IDisposable _ = ConsoleHelpers.WithError();

                Console.WriteLine("More than one solution (.sln) files were found in the current directory. Use the -p or --path paramter to specify one:");
                foreach (string solutionFile in solutionFiles)
                {
                    Console.WriteLine($" - '{solutionFile}'");
                }

                return false;
            }

            solutionFilePath = solutionFiles[0];
        }
        else
        {
            if (fileSystem.Path.IsPathFullyQualified(input))
            {
                solutionFilePath = input;
            }
            else
            {
                // Assume it is a relative path to the current directory.
                solutionFilePath = fileSystem.Path.GetFullPath(input, currentDirectory);
            }
        }

        return solutionFilePath is not null;
    }
}
