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
        Assert.Equal(SolutionFileHeader.SupportedFileFormatVersion, fileHeader.FileFormatVersion);
        Assert.Equal(16, fileHeader.LastVisualStudioMajorVersion);
        Assert.Equal(expectedVersion, fileHeader.LastVisualStudioVersion);
        Assert.Equal(SolutionFileBuilder.DefaultVisualStudioMinimumVersion, fileHeader.MinimumVisualStudioVersion);
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
        Assert.Equal(SolutionFileHeader.SupportedFileFormatVersion, fileHeader.FileFormatVersion);
        Assert.Equal(15, fileHeader.LastVisualStudioMajorVersion);
        Assert.Equal(expectedVersion, fileHeader.LastVisualStudioVersion);
        Assert.Equal(expectedVersion, fileHeader.MinimumVisualStudioVersion);
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
        Assert.Equal(SolutionFileHeader.SupportedFileFormatVersion, fileHeader.FileFormatVersion);
        Assert.Null(fileHeader.LastVisualStudioMajorVersion);
        Assert.Null(fileHeader.LastVisualStudioVersion);
        Assert.Null(fileHeader.MinimumVisualStudioVersion);
    }

    [Fact]
    public void Construct_WithMinimalHeaderNoBody()
    {
        // Arrange
        MockFileSystem fileSystem = new SolutionFileBuilder()
            .ConfigureMinimumHeader()
            .ExcludeBody()
            .BuildToMockFileSystem(out string filePath);

        // Act
        SolutionFile solutionFile = new(fileSystem, filePath);

        // Assert
        SolutionFileHeader fileHeader = solutionFile.FileHeader;
        Assert.Equal(SolutionFileHeader.SupportedFileFormatVersion, fileHeader.FileFormatVersion);
        Assert.Null(fileHeader.LastVisualStudioMajorVersion);
        Assert.Null(fileHeader.LastVisualStudioVersion);
        Assert.Null(fileHeader.MinimumVisualStudioVersion);
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
        Assert.Equal(filePath, exception.FileName);
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
        Assert.Equal(SolutionFileHeader.SupportedFileFormatVersion, fileHeader.FileFormatVersion);
        Assert.Equal(expectedVersion.Major, fileHeader.LastVisualStudioMajorVersion);
        Assert.Equal(expectedVersion, fileHeader.LastVisualStudioVersion);
        Assert.Equal(SolutionFileBuilder.DefaultVisualStudioMinimumVersion, fileHeader.MinimumVisualStudioVersion);
        Assert.Equal(expectedContent, solutionFile.ReadContent());
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
        Assert.Equal(SolutionFileHeader.SupportedFileFormatVersion, fileHeader.FileFormatVersion);
        Assert.Equal(expectedVersion.Major, fileHeader.LastVisualStudioMajorVersion);
        Assert.Equal(expectedVersion, fileHeader.LastVisualStudioVersion);
        Assert.Equal(SolutionFileBuilder.DefaultVisualStudioMinimumVersion, fileHeader.MinimumVisualStudioVersion);
        Assert.Equal(expectedContent, solutionFile.ReadContent());
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
        Assert.Equal(SolutionFileHeader.SupportedFileFormatVersion, fileHeader.FileFormatVersion);
        Assert.Equal(expectedVersion.Major, fileHeader.LastVisualStudioMajorVersion);
        Assert.Equal(expectedVersion, fileHeader.LastVisualStudioVersion);
        Assert.Equal(Version.Parse(SolutionFileHeader.DefaultMinimumVisualStudioVersion), fileHeader.MinimumVisualStudioVersion);
        Assert.Equal(expectedFileContent, solutionFile.ReadContent());
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
        Assert.Equal(SolutionFileHeader.SupportedFileFormatVersion, fileHeader.FileFormatVersion);
        Assert.Equal(expectedVersion.Major, fileHeader.LastVisualStudioMajorVersion);
        Assert.Equal(expectedVersion, fileHeader.LastVisualStudioVersion);
        Assert.Equal(Version.Parse(SolutionFileHeader.DefaultMinimumVisualStudioVersion), fileHeader.MinimumVisualStudioVersion);
        Assert.Equal(expectedFileContent, solutionFile.ReadContent());
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
        Assert.Equal(SolutionFileHeader.SupportedFileFormatVersion, fileHeader.FileFormatVersion);
        Assert.Equal(expectedVersion.Major, fileHeader.LastVisualStudioMajorVersion);
        Assert.Equal(expectedVersion, fileHeader.LastVisualStudioVersion);
        Assert.Equal(Version.Parse(SolutionFileHeader.DefaultMinimumVisualStudioVersion), fileHeader.MinimumVisualStudioVersion);
        Assert.Equal(expectedFileContent, solutionFile.ReadContent());
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
        Assert.Equal(SolutionFileHeader.SupportedFileFormatVersion, fileHeader.FileFormatVersion);
        Assert.Equal(expectedVersion.Major, fileHeader.LastVisualStudioMajorVersion);
        Assert.Equal(expectedVersion, fileHeader.LastVisualStudioVersion);
        Assert.Equal(Version.Parse(SolutionFileHeader.DefaultMinimumVisualStudioVersion), fileHeader.MinimumVisualStudioVersion);
        Assert.Equal(expectedFileContent, solutionFile.ReadContent());
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
