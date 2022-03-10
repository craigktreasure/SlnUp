namespace SlnUp;

internal record SolutionFileHeader
{
    public const string SupportedFileFormatVersion = "12.00";

    public const string DefaultMinimumVisualStudioVersion = "10.0.40219.1";

    public string FileFormatVersion { get; init; }

    public int? LastVisualStudioMajorVersion { get; init; }

    public Version? LastVisualStudioVersion { get; init; }

    public Version? MinimumVisualStudioVersion { get; init; }

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

    public SolutionFileHeader DuplicateAndUpdate(Version newVisualStudioVersion) => this with
    {
        LastVisualStudioMajorVersion = newVisualStudioVersion.Major,
        LastVisualStudioVersion = newVisualStudioVersion,
    };
}
