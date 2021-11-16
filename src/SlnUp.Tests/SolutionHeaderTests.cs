namespace SlnUp.Tests;

using FluentAssertions;
using Xunit;

public class SolutionHeaderTests
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
    public void ConstructFromVersion()
    {
        // Arrange
        Version version = Version.Parse("17.0.31903.59");

        // Act
        SolutionFileHeader fileHeader = new(version);

        // Assert
        fileHeader.FileFormatVersion.Should().Be(SolutionFileHeader.SupportedFileFormatVersion);
        fileHeader.LastVisualStudioMajorVersion.Should().Be(version.Major);
        fileHeader.LastVisualStudioVersion.Should().Be(version);
        fileHeader.MinimumVisualStudioVersion.Should().Be(Version.Parse(SolutionFileHeader.DefaultMinimumVisualStudioVersion));
    }

    [Fact]
    public void ConstructWithUnsupportedFileFormatVersion()
    {
        // Act and assert
        ArgumentException ex = Assert.Throws<ArgumentException>(() => new SolutionFileHeader("13.00"));

        // Assert
        ex.ParamName.Should().Be("fileFormatVersion");
    }
}
