namespace SlnUp.Tests;

using System.IO.Abstractions.TestingHelpers;

using SlnUp.TestLibrary;

public class SolutionFileTests
{
    [Fact]
    public void Construct_WithEmptyFile()
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
    public void Construct_WithFullHeader()
    {
        // Arrange
        Version expectedVersion = Version.Parse("16.0.30114.105");
        MockFileSystem fileSystem = new SolutionFileBuilder(expectedVersion)
            .BuildToMockFileSystem(out string filePath);

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
    public void Construct_WithFullV15Header()
    {
        // Arrange
        Version expectedVersion = Version.Parse("15.0.26124.0");
        MockFileSystem fileSystem = new SolutionFileBuilder(expectedVersion, visualStudioMinimumVersion: expectedVersion)
            .BuildToMockFileSystem(out string filePath);

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
    public void Construct_WithMinimalHeader()
    {
        // Arrange
        MockFileSystem fileSystem = new SolutionFileBuilder()
            .ConfigureMinimumHeader()
            .BuildToMockFileSystem(out string filePath);

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
    public void Construct_WithMissingFile()
    {
        // Arrange
        MockFileSystem fileSystem = new();
        string filePath = TemporaryFile.GetRandomFilePathWithExtension(fileSystem, "sln");

        // Act and assert
        FileNotFoundException exception = Assert.Throws<FileNotFoundException>(() => new SolutionFile(fileSystem, filePath));

        // Assert
        exception.FileName.Should().Be(filePath);
    }

    [Fact]
    public void Construct_WithMissingFileFormatVersion()
    {
        // Arrange
        MockFileSystem fileSystem = new SolutionFileBuilder()
            .ExcludeFileFormatVersion()
            .BuildToMockFileSystem(out string filePath);

        // Act
        Assert.Throws<InvalidDataException>(() => new SolutionFile(fileSystem, filePath));
    }

    [Fact]
    public void UpdateFileHeader_WithFullHeader()
    {
        // Arrange
        SolutionFile solutionFile = new SolutionFileBuilder(Version.Parse("16.0.30114.105"))
            .BuildToMockSolutionFile();
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
    public void UpdateFileHeader_WithFullV15Header()
    {
        // Arrange
        SolutionFile solutionFile = new SolutionFileBuilder(Version.Parse("15.0.26124.0"))
            .BuildToMockSolutionFile();
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
    public void UpdateFileHeader_WithMinimalHeader()
    {
        // Arrange
        Version expectedVersion = Version.Parse("17.0.31903.59");
        string expectedFileContent = new SolutionFileBuilder(expectedVersion).Build();
        SolutionFile solutionFile = new SolutionFileBuilder()
            .ConfigureMinimumHeader()
            .BuildToMockSolutionFile();

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
    public void UpdateFileHeader_WithMissingMajorVersion()
    {
        // Arrange
        Version expectedVersion = Version.Parse("17.0.31903.59");
        string expectedFileContent = new SolutionFileBuilder(expectedVersion).Build();
        SolutionFile solutionFile = new SolutionFileBuilder(expectedVersion)
                .ExcludeSolutionIconVersion()
                .BuildToMockSolutionFile();

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
    public void UpdateFileHeader_WithMissingMinimumVersion()
    {
        // Arrange
        Version expectedVersion = Version.Parse("17.0.31903.59");
        string expectedFileContent = new SolutionFileBuilder(expectedVersion).Build();
        SolutionFile solutionFile = new SolutionFileBuilder(expectedVersion)
            .ExcludeVisualStudioMinimumVersion()
            .BuildToMockSolutionFile();

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
    public void UpdateFileHeader_WithMissingVersion()
    {
        // Arrange
        Version expectedVersion = Version.Parse("17.0.31903.59");
        string expectedFileContent = new SolutionFileBuilder(expectedVersion).Build();
        SolutionFile solutionFile = new SolutionFileBuilder(expectedVersion)
            .ExcludeVisualStudioFullVersion()
            .BuildToMockSolutionFile();

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
    public void UpdateFileHeader_WithNullLastVisualStudioMajorVersion()
    {
        // Arrange
        Version expectedVersion = Version.Parse("17.0.31903.59");
        SolutionFile solutionFile = new SolutionFileBuilder(expectedVersion).BuildToMockSolutionFile();
        SolutionFileHeader fileHeader = new(
            fileFormatVersion: SolutionFileHeader.SupportedFileFormatVersion,
            lastVisualStudioMajorVersion: null,
            lastVisualStudioVersion: expectedVersion,
            minimumVisualStudioVersion: expectedVersion);

        // Act and assert
        Assert.Throws<InvalidDataException>(() => solutionFile.UpdateFileHeader(fileHeader));
    }

    [Fact]
    public void UpdateFileHeader_WithNullLastVisualStudioVersion()
    {
        // Arrange
        Version expectedVersion = Version.Parse("17.0.31903.59");
        SolutionFile solutionFile = new SolutionFileBuilder(expectedVersion).BuildToMockSolutionFile();
        SolutionFileHeader fileHeader = new(
            fileFormatVersion: SolutionFileHeader.SupportedFileFormatVersion,
            lastVisualStudioMajorVersion: expectedVersion.Major,
            lastVisualStudioVersion: null,
            minimumVisualStudioVersion: expectedVersion);

        // Act and assert
        Assert.Throws<InvalidDataException>(() => solutionFile.UpdateFileHeader(fileHeader));
    }
}
