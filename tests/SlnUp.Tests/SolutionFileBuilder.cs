namespace SlnUp.Tests;

using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text;

using SlnUp.TestLibrary;

internal sealed class SolutionFileBuilder
{
    public static readonly Version DefaultVisualStudioFullVersion = Version.Parse("16.0.28701.123");

    public static readonly Version DefaultVisualStudioMinimumVersion = Version.Parse(SolutionFileHeader.DefaultMinimumVisualStudioVersion);

    private readonly string fileFormatVersion;

    private readonly int iconMajorVersion;

    private readonly Version visualStudioFullVersion;

    private readonly Version visualStudioMinimumVersion;

    private bool includeBody = true;

    private bool includeFileFormatVersion = true;

    private bool includeSolutionIconVersion = true;

    private bool includeVisualStudioFullVersion = true;

    private bool includeVisualStudioMinimumVersion = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="SolutionFileBuilder"/> class using all default values.
    /// </summary>
    public SolutionFileBuilder()
        : this(DefaultVisualStudioFullVersion) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SolutionFileBuilder"/> class from a full Visual Studio version
    /// and optional file format version and mimimum Visual Studio version.
    /// </summary>
    /// <param name="visualStudioFullVersion">The full Visual Studio version.</param>
    /// <param name="fileFormatVersion">The file format version.</param>
    /// <param name="visualStudioMinimumVersion">The minimum Visual Studio version.</param>
    public SolutionFileBuilder(Version visualStudioFullVersion, string fileFormatVersion = SolutionFileHeader.SupportedFileFormatVersion, Version? visualStudioMinimumVersion = null)
        : this(fileFormatVersion, visualStudioFullVersion.Major, visualStudioFullVersion, visualStudioMinimumVersion ?? DefaultVisualStudioMinimumVersion) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SolutionFileBuilder"/> class.
    /// </summary>
    /// <param name="fileFormatVersion">The file format version.</param>
    /// <param name="iconMajorVersion">The icon major version.</param>
    /// <param name="visualStudioFullVersion">The full Visual Studio version.</param>
    /// <param name="visualStudioMinimumVersion">The minimum Visual Studio version.</param>
    public SolutionFileBuilder(string fileFormatVersion, int iconMajorVersion, Version visualStudioFullVersion, Version visualStudioMinimumVersion)
    {
        this.fileFormatVersion = fileFormatVersion;
        this.iconMajorVersion = iconMajorVersion;
        this.visualStudioFullVersion = visualStudioFullVersion;
        this.visualStudioMinimumVersion = visualStudioMinimumVersion;
    }

    /// <summary>
    /// Builds the content of the solution file.
    /// </summary>
    /// <returns><see cref="string"/>.</returns>
    public string Build()
    {
        StringBuilder builder = new();

        builder.AppendLine();

        if (this.includeFileFormatVersion)
        {
            builder.Append("Microsoft Visual Studio Solution File, Format Version ")
                .AppendLine(this.fileFormatVersion);
        }

        if (this.includeSolutionIconVersion)
        {
            if (this.iconMajorVersion >= 16)
            {
                builder.Append("# Visual Studio Version ").Append(this.iconMajorVersion).AppendLine();
            }
            else
            {
                builder.Append("# Visual Studio ").Append(this.iconMajorVersion).AppendLine();
            }
        }

        if (this.includeVisualStudioFullVersion)
        {
            builder.Append("VisualStudioVersion = ").Append(this.visualStudioFullVersion).AppendLine();
        }

        if (this.includeVisualStudioMinimumVersion)
        {
            builder.Append("MinimumVisualStudioVersion = ").Append(this.visualStudioMinimumVersion).AppendLine();
        }

        if (this.includeBody)
        {
            builder.AppendLine(@"
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
EndGlobal
".Trim());
        }

        return builder.ToString();
    }

    /// <summary>
    /// Builds a solution file and write it to the specified file path in the specified file system.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="filePath">The file path.</param>
    public void BuildToFile(IFileSystem fileSystem, string filePath)
        => fileSystem.File.WriteAllText(filePath, this.Build());

    /// <summary>
    /// Builds the content of the solution file and puts it into a <see cref="MockFileSystem"/>.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <returns><see cref="MockFileSystem"/>.</returns>
    public MockFileSystem BuildToMockFileSystem(out string filePath)
    {
        MockFileSystem fileSystem = new();
        filePath = TemporaryFile.GetRandomFilePathWithExtension(fileSystem, "sln");
        this.BuildToFile(fileSystem, filePath);

        return fileSystem;
    }

    /// <summary>
    /// Builds the content to a <see cref="SolutionFile"/>.
    /// </summary>
    /// <returns><see cref="SolutionFile"/>.</returns>
    public SolutionFile BuildToMockSolutionFile()
    {
        IFileSystem fileSystem = this.BuildToMockFileSystem(out string filePath);
        SolutionFile solutionFile = new(fileSystem, filePath);

        return solutionFile;
    }

    /// <summary>
    /// Builds the content to a <see cref="SolutionFile" />.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="filePath">The file path.</param>
    /// <returns><see cref="SolutionFile" />.</returns>
    public SolutionFile BuildToSolutionFile(IFileSystem fileSystem, string filePath)
    {
        this.BuildToFile(fileSystem, filePath);
        SolutionFile solutionFile = new(fileSystem, filePath);

        return solutionFile;
    }

    /// <summary>
    /// Configures the minimum file header lacking the solution icon version and full and minimum Visual Studio versions.
    /// </summary>
    /// <returns><see cref="SolutionFileBuilder"/>.</returns>
    public SolutionFileBuilder ConfigureMinimumHeader()
    {
        this.includeSolutionIconVersion = false;
        this.includeVisualStudioFullVersion = false;
        this.includeVisualStudioMinimumVersion = false;

        return this;
    }

    /// <summary>
    /// Excludes the body from the generated file content.
    /// </summary>
    /// <returns><see cref="SolutionFileBuilder"/>.</returns>
    public SolutionFileBuilder ExcludeBody()
    {
        this.includeBody = false;

        return this;
    }

    /// <summary>
    /// Excludes the file format version from the generated file content.
    /// </summary>
    /// <returns><see cref="SolutionFileBuilder"/>.</returns>
    public SolutionFileBuilder ExcludeFileFormatVersion()
    {
        this.includeFileFormatVersion = false;

        return this;
    }

    /// <summary>
    /// Excludes the solution icon version from the generated file content.
    /// </summary>
    /// <returns><see cref="SolutionFileBuilder"/>.</returns>
    public SolutionFileBuilder ExcludeSolutionIconVersion()
    {
        this.includeSolutionIconVersion = false;

        return this;
    }

    /// <summary>
    /// Excludes the full Visual Studio version from the generated file content.
    /// </summary>
    /// <returns><see cref="SolutionFileBuilder"/>.</returns>
    public SolutionFileBuilder ExcludeVisualStudioFullVersion()
    {
        this.includeVisualStudioFullVersion = false;

        return this;
    }

    /// <summary>
    /// Excludes the minimum Visual Studio version from the generated file content.
    /// </summary>
    /// <returns><see cref="SolutionFileBuilder"/>.</returns>
    public SolutionFileBuilder ExcludeVisualStudioMinimumVersion()
    {
        this.includeVisualStudioMinimumVersion = false;

        return this;
    }
}
