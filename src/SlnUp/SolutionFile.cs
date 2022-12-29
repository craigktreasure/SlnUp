namespace SlnUp;

using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Text.RegularExpressions;

using SlnUp.Core.Extensions;

internal class SolutionFile
{
    private static readonly Regex fileFormatVersionRegex = new(
        @"^Microsoft Visual Studio Solution File, Format Version (\d+\.\d+)$",
        RegexOptions.Compiled);

    private static readonly Regex lastVisualStudioMajorVersionRegex = new(
        @"^# Visual Studio(?: Version)? (\d+)$",
        RegexOptions.Compiled);

    private static readonly Regex lastVisualStudioVersionRegex = new(
        @"^VisualStudioVersion = (\d+\.\d+\.\d+\.\d+)$",
        RegexOptions.Compiled);

    private static readonly Regex minimumVisualStudioVersionRegex = new(
        @"^MinimumVisualStudioVersion = (\d+\.\d+\.\d+\.\d+)$",
        RegexOptions.Compiled);

    private readonly string filePath;

    private readonly IFileSystem fileSystem;

    private int fileFormatLineNumber = -1;

    /// <summary>
    /// Gets the file header.
    /// </summary>
    public SolutionFileHeader FileHeader { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SolutionFile"/> class.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="filePath">The file path.</param>
    /// <exception cref="FileNotFoundException">The solution file could not be found.</exception>
    public SolutionFile(IFileSystem fileSystem, string filePath)
    {
        if (!fileSystem.File.Exists(filePath))
        {
            throw new FileNotFoundException("The solution file could not be found.", filePath);
        }

        this.fileSystem = fileSystem;
        this.filePath = filePath;

        this.FileHeader = this.LoadFileHeader();
    }

    /// <summary>
    /// Reads the file content.
    /// </summary>
    /// <returns><see cref="string"/>.</returns>
    public string ReadContent() => this.fileSystem.File.ReadAllText(this.filePath);

    /// <summary>
    /// Reload.
    /// </summary>
    public void Reload()
        => this.FileHeader = this.LoadFileHeader();

    /// <summary>
    /// Updates the file header.
    /// </summary>
    /// <param name="newVisualStudioVersion">The new Visual Studio version.</param>
    public void UpdateFileHeader(Version newVisualStudioVersion)
    {
        SolutionFileHeader newHeader = this.FileHeader.DuplicateAndUpdate(newVisualStudioVersion);
        this.UpdateFileHeader(newHeader);
    }

    internal void UpdateFileHeader(SolutionFileHeader fileHeader)
    {
        if (fileHeader.LastVisualStudioMajorVersion is null)
        {
            throw new InvalidDataException($"The {nameof(SolutionFileHeader.LastVisualStudioMajorVersion)} cannot be null.");
        }

        if (fileHeader.LastVisualStudioVersion is null)
        {
            throw new InvalidDataException($"The {nameof(SolutionFileHeader.LastVisualStudioVersion)} cannot be null.");
        }

        if (fileHeader.MinimumVisualStudioVersion is null)
        {
            // Inject a default minimum Visual Studio version.
            fileHeader = fileHeader with
            {
                MinimumVisualStudioVersion = Version.Parse(SolutionFileHeader.DefaultMinimumVisualStudioVersion)
            };
        }

        List<string> lines = this.fileSystem.File.ReadAllLines(this.filePath).ToList();

        if (this.FileHeader.LastVisualStudioMajorVersion is null)
        {
            lines.Insert(this.fileFormatLineNumber + 1, string.Empty);
        }

        if (this.FileHeader.LastVisualStudioVersion is null)
        {
            lines.Insert(this.fileFormatLineNumber + 2, string.Empty);
        }

        if (this.FileHeader.MinimumVisualStudioVersion is null)
        {
            lines.Insert(this.fileFormatLineNumber + 3, string.Empty);
        }

        string fileFormatVersionLine = $"Microsoft Visual Studio Solution File, Format Version {fileHeader.FileFormatVersion}";
        lines[this.fileFormatLineNumber] = fileFormatVersionLine;

#pragma warning disable IDE0072 // Add missing cases
        string lastVisualStudioMajorVersionLine = fileHeader.LastVisualStudioMajorVersion switch
        {
            >= 16 => $"# Visual Studio Version {fileHeader.LastVisualStudioMajorVersion}",
            <= 15 => $"# Visual Studio {fileHeader.LastVisualStudioMajorVersion}",
        };
#pragma warning restore IDE0072 // Add missing cases
        lines[this.fileFormatLineNumber + 1] = lastVisualStudioMajorVersionLine;

        string lastVisualStudioVersionLine = $"VisualStudioVersion = {fileHeader.LastVisualStudioVersion}";
        lines[this.fileFormatLineNumber + 2] = lastVisualStudioVersionLine;

        string minimumVisualStudioVersionLine = $"MinimumVisualStudioVersion = {fileHeader.MinimumVisualStudioVersion}";
        lines[this.fileFormatLineNumber + 3] = minimumVisualStudioVersionLine;

        this.fileSystem.File.WriteAllLines(this.filePath, lines);

        this.FileHeader = fileHeader;
    }

    private static bool TryGetFileFormat(string line, [NotNullWhen(true)] out string? fileFormatVersion)
    {
        fileFormatVersion = null;

        if (fileFormatVersionRegex.TryMatch(line, out Match? fileFormatVersionMatch))
        {
            fileFormatVersion = fileFormatVersionMatch.Groups[1].Value;
        }

        return fileFormatVersion is not null;
    }

    private static bool TryGetLastVisualStudioMajorVersion(string line, [NotNullWhen(true)] out int? lastMajorVersion)
    {
        lastMajorVersion = null;

        if (lastVisualStudioMajorVersionRegex.TryMatch(line, out Match? majorVersionMatch)
            && int.TryParse(majorVersionMatch.Groups[1].Value, out int parsedLastMajorVersion))
        {
            lastMajorVersion = parsedLastMajorVersion;
        }

        return lastMajorVersion is not null;
    }

    private static bool TryGetLastVisualStudioVersion(string line, [NotNullWhen(true)] out Version? lastVersion)
    {
        lastVersion = null;

        if (lastVisualStudioVersionRegex.TryMatch(line, out Match? majorVersionMatch)
            && Version.TryParse(majorVersionMatch.Groups[1].Value, out Version? parsedLastVersion))
        {
            lastVersion = parsedLastVersion;
        }

        return lastVersion is not null;
    }

    private static bool TryGetMinimumVisualStudioVersion(string line, [NotNullWhen(true)] out Version? minimumVersion)
    {
        minimumVersion = null;

        if (minimumVisualStudioVersionRegex.TryMatch(line, out Match? minimumVersionMatch)
            && Version.TryParse(minimumVersionMatch.Groups[1].Value, out Version? parsedMinimumVersion))
        {
            minimumVersion = parsedMinimumVersion;
        }

        return minimumVersion is not null;
    }

    private static bool TryLocateFileFormatLine(
        IReadOnlyList<string> lines,
        out int fileFormatLineNumber,
        [NotNullWhen(true)] out string? fileFormatVersion)
    {
        fileFormatLineNumber = -1;
        fileFormatVersion = null;

        for (int i = 0; i < lines.Count; ++i)
        {
            if (TryGetFileFormat(lines[i], out fileFormatVersion))
            {
                fileFormatLineNumber = i;
                break;
            }
        }

        return fileFormatLineNumber != -1;
    }

    private SolutionFileHeader LoadFileHeader()
    {
        IReadOnlyList<string> lines = this.fileSystem.File.ReadAllLines(this.filePath);

        if (!TryLocateFileFormatLine(lines, out this.fileFormatLineNumber, out string? fileFormatVersion))
        {
            throw new InvalidDataException($"The file does not contain a valid file format: '{this.filePath}'.");
        }

        SolutionFileHeader fileHeader = new(fileFormatVersion);

        int nextLine = this.fileFormatLineNumber + 1;
        if (lines.Count > nextLine + 1
            && TryGetLastVisualStudioMajorVersion(lines[nextLine], out int? lastMajorVersion))
        {
            fileHeader = fileHeader with
            {
                LastVisualStudioMajorVersion = lastMajorVersion,
            };
            ++nextLine;
        }

        if (lines.Count > nextLine + 1
            && TryGetLastVisualStudioVersion(lines[nextLine], out Version? lastVersion))
        {
            fileHeader = fileHeader with
            {
                LastVisualStudioVersion = lastVersion,
            };
            ++nextLine;
        }

        if (lines.Count > nextLine + 1
            && TryGetMinimumVisualStudioVersion(lines[nextLine], out Version? minimumVersion))
        {
            fileHeader = fileHeader with
            {
                MinimumVisualStudioVersion = minimumVersion,
            };
        }

        return fileHeader;
    }
}
