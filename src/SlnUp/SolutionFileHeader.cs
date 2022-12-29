namespace SlnUp;

internal record SolutionFileHeader
{
    public const string SupportedFileFormatVersion = "12.00";

    public const string DefaultMinimumVisualStudioVersion = "10.0.40219.1";

    /// <summary>
    /// Gets or sets the file format version.
    /// </summary>
    public string FileFormatVersion { get; init; }

    /// <summary>
    /// Gets or sets the last Visual Studio major version.
    /// </summary>
    public int? LastVisualStudioMajorVersion { get; init; }

    /// <summary>
    /// Gets or sets the last Visual Studio version.
    /// </summary>
    public Version? LastVisualStudioVersion { get; init; }

    /// <summary>
    /// Gets or sets the minimum Visual Studio version.
    /// </summary>
    public Version? MinimumVisualStudioVersion { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SolutionFileHeader"/> class.
    /// </summary>
    /// <param name="fileFormatVersion">The file format version.</param>
    /// <param name="lastVisualStudioMajorVersion">The last Visual Studio major version.</param>
    /// <param name="lastVisualStudioVersion">The last Visual Studio version.</param>
    /// <param name="minimumVisualStudioVersion">The minimum Visual Studio version.</param>
    /// <exception cref="ArgumentException">Only file format version {SupportedFileFormatVersion} is supported. - fileFormatVersion</exception>
    public SolutionFileHeader(
        string fileFormatVersion,
        int? lastVisualStudioMajorVersion = null,
        Version? lastVisualStudioVersion = null,
        Version? minimumVisualStudioVersion = null)
    {
        if (fileFormatVersion != SupportedFileFormatVersion)
        {
            throw new ArgumentException($"Only file format version {SupportedFileFormatVersion} is supported.", nameof(fileFormatVersion));
        }

        this.FileFormatVersion = fileFormatVersion;
        this.LastVisualStudioMajorVersion = lastVisualStudioMajorVersion;
        this.LastVisualStudioVersion = lastVisualStudioVersion;
        this.MinimumVisualStudioVersion = minimumVisualStudioVersion;
    }

    /// <summary>
    /// Duplicates this instance and updates with the specified version information.
    /// </summary>
    /// <param name="newVisualStudioVersion">The new visual studio version.</param>
    /// <returns><see cref="SolutionFileHeader"/>.</returns>
    public SolutionFileHeader DuplicateAndUpdate(Version newVisualStudioVersion) => this with
    {
        LastVisualStudioMajorVersion = newVisualStudioVersion.Major,
        LastVisualStudioVersion = newVisualStudioVersion,
    };
}
