namespace SlnUp.Tests;

public class SolutionFileHeaderTests
{
    [Test]
    public async Task Construct()
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
        await Assert.That(fileHeader.FileFormatVersion).IsEqualTo(SolutionFileHeader.SupportedFileFormatVersion);
        await Assert.That(fileHeader.LastVisualStudioMajorVersion).IsEqualTo(version.Major);
        await Assert.That(fileHeader.LastVisualStudioVersion).IsEqualTo(version);
        await Assert.That(fileHeader.MinimumVisualStudioVersion).IsEqualTo(Version.Parse(SolutionFileHeader.DefaultMinimumVisualStudioVersion));
    }

    [Test]
    public async Task Construct_WithUnsupportedFileFormatVersion()
    {
        // Arrange and act
        ArgumentException ex = Assert.Throws<ArgumentException>(() => _ = new SolutionFileHeader("13.00"));

        // Assert
        await Assert.That(ex.ParamName).IsEqualTo("fileFormatVersion");
    }

    [Test]
    public async Task DuplicateAndUpdate()
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
        await Assert.That(updatedFileHeader).IsNotSameReferenceAs(originalFileHeader);
        await Assert.That(updatedFileHeader.FileFormatVersion).IsEqualTo(SolutionFileHeader.SupportedFileFormatVersion);
        await Assert.That(updatedFileHeader.LastVisualStudioMajorVersion).IsEqualTo(updatedVersion.Major);
        await Assert.That(updatedFileHeader.LastVisualStudioVersion).IsEqualTo(updatedVersion);
        await Assert.That(updatedFileHeader.MinimumVisualStudioVersion).IsEqualTo(originalVersion);
    }
}
