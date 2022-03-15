namespace SlnUp.Tests;

using FluentAssertions;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

public class SolutionFileTests
{
    [Fact]
    public void ConstructWithEmptyFile()
    {
        // Arrange
        const string filePath = "C:\\MyProject.sln";
        MockFileSystem fileSystem = new(new Dictionary<string, MockFileData>
        {
            [filePath] = new MockFileData(string.Empty)
        });

        // Act and assert
        Assert.Throws<InvalidDataException>(() => new SolutionFile(fileSystem, filePath));
    }

    [Fact]
    public void ConstructWithFullHeader()
    {
        // Arrange
        Version expectedVersion = Version.Parse("16.0.30114.105");
        MockFileSystem fileSystem = new SolutionFileBuilder(expectedVersion)
            .BuildToFileSystem(out string filePath);

        // Act
        SolutionFile solutionFile = new(fileSystem, filePath);

        // Assert
        SolutionFileHeader fileHeader = solutionFile.FileHeader;
        fileHeader.FileFormatVersion.Should().Be(SolutionFileHeader.SupportedFileFormatVersion);
        fileHeader.LastVisualStudioMajorVersion.Should().Be(16);
        fileHeader.LastVisualStudioVersion.Should().Be(expectedVersion);
        fileHeader.MinimumVisualStudioVersion.Should().Be(SolutionFileBuilder.DefaultVisualStudioMinimumVersion);
    }

    [Fact]
    public void ConstructWithFullV15Header()
    {
        // Arrange
        Version expectedVersion = Version.Parse("15.0.26124.0");
        MockFileSystem fileSystem = new SolutionFileBuilder(expectedVersion, visualStudioMinimumVersion: expectedVersion)
            .BuildToFileSystem(out string filePath);

        // Act
        SolutionFile solutionFile = new(fileSystem, filePath);

        // Assert
        SolutionFileHeader fileHeader = solutionFile.FileHeader;
        fileHeader.FileFormatVersion.Should().Be(SolutionFileHeader.SupportedFileFormatVersion);
        fileHeader.LastVisualStudioMajorVersion.Should().Be(15);
        fileHeader.LastVisualStudioVersion.Should().Be(expectedVersion);
        fileHeader.MinimumVisualStudioVersion.Should().Be(expectedVersion);
    }

    [Fact]
    public void ConstructWithMinimalHeader()
    {
        // Arrange
        MockFileSystem fileSystem = new SolutionFileBuilder()
            .ConfigureMinimumHeader()
            .BuildToFileSystem(out string filePath);

        // Act
        SolutionFile solutionFile = new(fileSystem, filePath);

        // Assert
        SolutionFileHeader fileHeader = solutionFile.FileHeader;
        fileHeader.FileFormatVersion.Should().Be(SolutionFileHeader.SupportedFileFormatVersion);
        fileHeader.LastVisualStudioMajorVersion.Should().BeNull();
        fileHeader.LastVisualStudioVersion.Should().BeNull();
        fileHeader.MinimumVisualStudioVersion.Should().BeNull();
    }

    [Fact]
    public void ConstructWithMissingFile()
    {
        // Arrange
        const string filePath = "C:\\Missing.sln";
        MockFileSystem fileSystem = new();

        // Act and assert
        FileNotFoundException exception = Assert.Throws<FileNotFoundException>(() => new SolutionFile(fileSystem, filePath));

        // Assert
        exception.FileName.Should().Be(filePath);
    }

    [Fact]
    public void ConstructWithMissingFileFormatVersion()
    {
        // Arrange
        MockFileSystem fileSystem = new SolutionFileBuilder()
            .ExcludeFileFormatVersion()
            .BuildToFileSystem(out string filePath);

        // Act
        Assert.Throws<InvalidDataException>(() => new SolutionFile(fileSystem, filePath));
    }

    [Fact]
    public void UpdateFileHeaderWithFullHeader()
    {
        // Arrange
        SolutionFile solutionFile = new SolutionFileBuilder(Version.Parse("16.0.30114.105"))
            .BuildToSolutionFile();
        Version expectedVersion = Version.Parse("17.0.31903.59");
        string expectedContent = new SolutionFileBuilder(expectedVersion).Build();

        // Act
        solutionFile.UpdateFileHeader(expectedVersion);

        // Assert
        SolutionFileHeader fileHeader = solutionFile.FileHeader;
        fileHeader.FileFormatVersion.Should().Be(SolutionFileHeader.SupportedFileFormatVersion);
        fileHeader.LastVisualStudioMajorVersion.Should().Be(expectedVersion.Major);
        fileHeader.LastVisualStudioVersion.Should().Be(expectedVersion);
        fileHeader.MinimumVisualStudioVersion.Should().Be(SolutionFileBuilder.DefaultVisualStudioMinimumVersion);
        solutionFile.ReadContent().Should().Be(expectedContent);
    }

    [Fact]
    public void UpdateFileHeaderWithFullV15Header()
    {
        // Arrange
        SolutionFile solutionFile = new SolutionFileBuilder(Version.Parse("15.0.26124.0"))
            .BuildToSolutionFile();
        Version expectedVersion = Version.Parse("15.0.27000.0");
        string expectedContent = new SolutionFileBuilder(expectedVersion).Build();

        // Act
        solutionFile.UpdateFileHeader(expectedVersion);

        // Assert
        SolutionFileHeader fileHeader = solutionFile.FileHeader;
        fileHeader.FileFormatVersion.Should().Be(SolutionFileHeader.SupportedFileFormatVersion);
        fileHeader.LastVisualStudioMajorVersion.Should().Be(expectedVersion.Major);
        fileHeader.LastVisualStudioVersion.Should().Be(expectedVersion);
        fileHeader.MinimumVisualStudioVersion.Should().Be(SolutionFileBuilder.DefaultVisualStudioMinimumVersion);
        solutionFile.ReadContent().Should().Be(expectedContent);
    }

    [Fact]
    public void UpdateFileHeaderWithMinimalHeader()
    {
        // Arrange
        Version expectedVersion = Version.Parse("17.0.31903.59");
        string expectedFileContent = new SolutionFileBuilder(expectedVersion).Build();
        SolutionFile solutionFile = new SolutionFileBuilder()
            .ConfigureMinimumHeader()
            .BuildToSolutionFile();

        // Act
        solutionFile.UpdateFileHeader(expectedVersion);

        // Assert
        SolutionFileHeader fileHeader = solutionFile.FileHeader;
        fileHeader.FileFormatVersion.Should().Be(SolutionFileHeader.SupportedFileFormatVersion);
        fileHeader.LastVisualStudioMajorVersion.Should().Be(expectedVersion.Major);
        fileHeader.LastVisualStudioVersion.Should().Be(expectedVersion);
        fileHeader.MinimumVisualStudioVersion.Should().Be(Version.Parse(SolutionFileHeader.DefaultMinimumVisualStudioVersion));
        solutionFile.ReadContent().Should().Be(expectedFileContent);
    }

    [Fact]
    public void UpdateFileHeaderWithMissingMajorVersion()
    {
        // Arrange
        Version expectedVersion = Version.Parse("17.0.31903.59");
        string expectedFileContent = new SolutionFileBuilder(expectedVersion).Build();
        SolutionFile solutionFile = new SolutionFileBuilder(expectedVersion)
                .ExcludeSolutionIconVersion()
                .BuildToSolutionFile();

        // Act
        solutionFile.UpdateFileHeader(expectedVersion);

        // Assert
        SolutionFileHeader fileHeader = solutionFile.FileHeader;
        fileHeader.FileFormatVersion.Should().Be(SolutionFileHeader.SupportedFileFormatVersion);
        fileHeader.LastVisualStudioMajorVersion.Should().Be(expectedVersion.Major);
        fileHeader.LastVisualStudioVersion.Should().Be(expectedVersion);
        fileHeader.MinimumVisualStudioVersion.Should().Be(Version.Parse(SolutionFileHeader.DefaultMinimumVisualStudioVersion));
        solutionFile.ReadContent().Should().Be(expectedFileContent);
    }

    [Fact]
    public void UpdateFileHeaderWithMissingMinimumVersion()
    {
        // Arrange
        Version expectedVersion = Version.Parse("17.0.31903.59");
        string expectedFileContent = new SolutionFileBuilder(expectedVersion).Build();
        SolutionFile solutionFile = new SolutionFileBuilder(expectedVersion)
            .ExcludeVisualStudioMinimumVersion()
            .BuildToSolutionFile();

        // Act
        solutionFile.UpdateFileHeader(expectedVersion);

        // Assert
        SolutionFileHeader fileHeader = solutionFile.FileHeader;
        fileHeader.FileFormatVersion.Should().Be(SolutionFileHeader.SupportedFileFormatVersion);
        fileHeader.LastVisualStudioMajorVersion.Should().Be(expectedVersion.Major);
        fileHeader.LastVisualStudioVersion.Should().Be(expectedVersion);
        fileHeader.MinimumVisualStudioVersion.Should().Be(Version.Parse(SolutionFileHeader.DefaultMinimumVisualStudioVersion));
        solutionFile.ReadContent().Should().Be(expectedFileContent);
    }

    [Fact]
    public void UpdateFileHeaderWithMissingVersion()
    {
        // Arrange
        Version expectedVersion = Version.Parse("17.0.31903.59");
        string expectedFileContent = new SolutionFileBuilder(expectedVersion).Build();
        SolutionFile solutionFile = new SolutionFileBuilder(expectedVersion)
            .ExcludeVisualStudioFullVersion()
            .BuildToSolutionFile();

        // Act
        solutionFile.UpdateFileHeader(expectedVersion);

        // Assert
        SolutionFileHeader fileHeader = solutionFile.FileHeader;
        fileHeader.FileFormatVersion.Should().Be(SolutionFileHeader.SupportedFileFormatVersion);
        fileHeader.LastVisualStudioMajorVersion.Should().Be(expectedVersion.Major);
        fileHeader.LastVisualStudioVersion.Should().Be(expectedVersion);
        fileHeader.MinimumVisualStudioVersion.Should().Be(Version.Parse(SolutionFileHeader.DefaultMinimumVisualStudioVersion));
        solutionFile.ReadContent().Should().Be(expectedFileContent);
    }

    [Fact]
    public void UpdateFileHeaderWithNullLastVisualStudioMajorVersion()
    {
        // Arrange
        Version expectedVersion = Version.Parse("17.0.31903.59");
        SolutionFile solutionFile = new SolutionFileBuilder(expectedVersion).BuildToSolutionFile();
        SolutionFileHeader fileHeader = new(
            fileFormatVersion: SolutionFileHeader.SupportedFileFormatVersion,
            lastVisualStudioMajorVersion: null,
            lastVisualStudioVersion: expectedVersion,
            minimumVisualStudioVersion: expectedVersion);

        // Act and assert
        Assert.Throws<InvalidDataException>(() => solutionFile.UpdateFileHeader(fileHeader));
    }

    [Fact]
    public void UpdateFileHeaderWithNullLastVisualStudioVersion()
    {
        // Arrange
        Version expectedVersion = Version.Parse("17.0.31903.59");
        SolutionFile solutionFile = new SolutionFileBuilder(expectedVersion).BuildToSolutionFile();
        SolutionFileHeader fileHeader = new(
            fileFormatVersion: SolutionFileHeader.SupportedFileFormatVersion,
            lastVisualStudioMajorVersion: expectedVersion.Major,
            lastVisualStudioVersion: null,
            minimumVisualStudioVersion: expectedVersion);

        // Act and assert
        Assert.Throws<InvalidDataException>(() => solutionFile.UpdateFileHeader(fileHeader));
    }
}
