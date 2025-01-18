namespace SlnUp.Tests;

public class SolutionFileHeaderTests
{
    [Fact]
    public void Construct()
    {
        // Arrange
        Version version = Version.Parse("17.0.31903.59");

        // Act
        SolutionFileHeader fileHeader = new(
            SolutionFileHeader.SupportedFileFormatVersion,
            version.Major,
            version,
            Version.Parse(SolutionFileHeader.DefaultMinimumVisualStudioVersion));

        // Assert
        Assert.Equal(SolutionFileHeader.SupportedFileFormatVersion, fileHeader.FileFormatVersion);
        Assert.Equal(version.Major, fileHeader.LastVisualStudioMajorVersion);
        Assert.Equal(version, fileHeader.LastVisualStudioVersion);
        Assert.Equal(Version.Parse(SolutionFileHeader.DefaultMinimumVisualStudioVersion), fileHeader.MinimumVisualStudioVersion);
    }

    [Fact]
    public void Construct_WithUnsupportedFileFormatVersion()
    {
        // Arrange and act
        ArgumentException ex = Assert.Throws<ArgumentException>(() => new SolutionFileHeader("13.00"));

        // Assert
        Assert.Equal("fileFormatVersion", ex.ParamName);
    }

    [Fact]
    public void DuplicateAndUpdate()
    {
        // Arrange
        Version originalVersion = Version.Parse("17.0.31903.59");
        SolutionFileHeader originalFileHeader = new(
            SolutionFileHeader.SupportedFileFormatVersion,
            originalVersion.Major,
            originalVersion,
            originalVersion);
        Version updatedVersion = Version.Parse("17.1.32210.238");

        // Act
        SolutionFileHeader updatedFileHeader = originalFileHeader.DuplicateAndUpdate(updatedVersion);

        // Assert
        Assert.NotSame(originalFileHeader, updatedFileHeader);
        Assert.Equal(SolutionFileHeader.SupportedFileFormatVersion, updatedFileHeader.FileFormatVersion);
        Assert.Equal(updatedVersion.Major, updatedFileHeader.LastVisualStudioMajorVersion);
        Assert.Equal(updatedVersion, updatedFileHeader.LastVisualStudioVersion);
        Assert.Equal(originalVersion, updatedFileHeader.MinimumVisualStudioVersion);
    }
}
