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
        fileHeader.FileFormatVersion.Should().Be(SolutionFileHeader.SupportedFileFormatVersion);
        fileHeader.LastVisualStudioMajorVersion.Should().Be(version.Major);
        fileHeader.LastVisualStudioVersion.Should().Be(version);
        fileHeader.MinimumVisualStudioVersion.Should().Be(Version.Parse(SolutionFileHeader.DefaultMinimumVisualStudioVersion));
    }

    [Fact]
    public void Construct_WithUnsupportedFileFormatVersion()
    {
        // Arrange and act
        ArgumentException ex = Assert.Throws<ArgumentException>(() => new SolutionFileHeader("13.00"));

        // Assert
        ex.ParamName.Should().Be("fileFormatVersion");
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
        updatedFileHeader.Should().NotBeSameAs(originalFileHeader);
        updatedFileHeader.FileFormatVersion.Should().Be(SolutionFileHeader.SupportedFileFormatVersion);
        updatedFileHeader.LastVisualStudioMajorVersion.Should().Be(updatedVersion.Major);
        updatedFileHeader.LastVisualStudioVersion.Should().Be(updatedVersion);
        updatedFileHeader.MinimumVisualStudioVersion.Should().Be(originalVersion);
    }
}
