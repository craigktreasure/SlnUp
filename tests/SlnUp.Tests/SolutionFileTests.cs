namespace SlnUp.Tests;

using System.IO.Abstractions.TestingHelpers;

using SlnUp.TestLibrary;

public class SolutionFileTests
{
    [Test]
    public void Construct_WithEmptyFile()
    {
        // Arrange
        const string filePath = "C:\\MyProject.sln";
        MockFileSystem fileSystem = new(new Dictionary<string, MockFileData>
        {
            [filePath] = new MockFileData(string.Empty)
        });

        // Act and assert
        Assert.Throws<InvalidDataException>(() => _ = new SolutionFile(fileSystem, filePath));
    }

    [Test]
    public async Task Construct_WithFullHeader()
    {
        // Arrange
        Version expectedVersion = Version.Parse("16.0.30114.105");
        MockFileSystem fileSystem = new SolutionFileBuilder(expectedVersion)
            .BuildToMockFileSystem(out string filePath);

        // Act
        SolutionFile solutionFile = new(fileSystem, filePath);

        // Assert
        SolutionFileHeader fileHeader = solutionFile.FileHeader;
        await Assert.That(fileHeader.FileFormatVersion).IsEqualTo(SolutionFileHeader.SupportedFileFormatVersion);
        await Assert.That(fileHeader.LastVisualStudioMajorVersion).IsEqualTo(16);
        await Assert.That(fileHeader.LastVisualStudioVersion).IsEqualTo(expectedVersion);
        await Assert.That(fileHeader.MinimumVisualStudioVersion).IsEqualTo(SolutionFileBuilder.DefaultVisualStudioMinimumVersion);
    }

    [Test]
    public async Task Construct_WithFullV15Header()
    {
        // Arrange
        Version expectedVersion = Version.Parse("15.0.26124.0");
        MockFileSystem fileSystem = new SolutionFileBuilder(expectedVersion, visualStudioMinimumVersion: expectedVersion)
            .BuildToMockFileSystem(out string filePath);

        // Act
        SolutionFile solutionFile = new(fileSystem, filePath);

        // Assert
        SolutionFileHeader fileHeader = solutionFile.FileHeader;
        await Assert.That(fileHeader.FileFormatVersion).IsEqualTo(SolutionFileHeader.SupportedFileFormatVersion);
        await Assert.That(fileHeader.LastVisualStudioMajorVersion).IsEqualTo(15);
        await Assert.That(fileHeader.LastVisualStudioVersion).IsEqualTo(expectedVersion);
        await Assert.That(fileHeader.MinimumVisualStudioVersion).IsEqualTo(expectedVersion);
    }

    [Test]
    public async Task Construct_WithMinimalHeader()
    {
        // Arrange
        MockFileSystem fileSystem = new SolutionFileBuilder()
            .ConfigureMinimumHeader()
            .BuildToMockFileSystem(out string filePath);

        // Act
        SolutionFile solutionFile = new(fileSystem, filePath);

        // Assert
        SolutionFileHeader fileHeader = solutionFile.FileHeader;
        await Assert.That(fileHeader.FileFormatVersion).IsEqualTo(SolutionFileHeader.SupportedFileFormatVersion);
        await Assert.That(fileHeader.LastVisualStudioMajorVersion).IsNull();
        await Assert.That(fileHeader.LastVisualStudioVersion).IsNull();
        await Assert.That(fileHeader.MinimumVisualStudioVersion).IsNull();
    }

    [Test]
    public async Task Construct_WithMinimalHeaderNoBody()
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
        await Assert.That(fileHeader.FileFormatVersion).IsEqualTo(SolutionFileHeader.SupportedFileFormatVersion);
        await Assert.That(fileHeader.LastVisualStudioMajorVersion).IsNull();
        await Assert.That(fileHeader.LastVisualStudioVersion).IsNull();
        await Assert.That(fileHeader.MinimumVisualStudioVersion).IsNull();
    }

    [Test]
    public async Task Construct_WithMissingFile()
    {
        // Arrange
        MockFileSystem fileSystem = new();
        string filePath = TemporaryFile.GetRandomFilePathWithExtension(fileSystem, "sln");

        // Act and assert
        FileNotFoundException exception = Assert.Throws<FileNotFoundException>(
            () => _ = new SolutionFile(fileSystem, filePath));

        // Assert
        await Assert.That(exception.FileName).IsEqualTo(filePath);
    }

    [Test]
    public void Construct_WithMissingFileFormatVersion()
    {
        // Arrange
        MockFileSystem fileSystem = new SolutionFileBuilder()
            .ExcludeFileFormatVersion()
            .BuildToMockFileSystem(out string filePath);

        // Act
        Assert.Throws<InvalidDataException>(() => _ = new SolutionFile(fileSystem, filePath));
    }

    [Test]
    public async Task UpdateFileHeader_WithFullHeader()
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
        await Assert.That(fileHeader.FileFormatVersion).IsEqualTo(SolutionFileHeader.SupportedFileFormatVersion);
        await Assert.That(fileHeader.LastVisualStudioMajorVersion).IsEqualTo(expectedVersion.Major);
        await Assert.That(fileHeader.LastVisualStudioVersion).IsEqualTo(expectedVersion);
        await Assert.That(fileHeader.MinimumVisualStudioVersion).IsEqualTo(SolutionFileBuilder.DefaultVisualStudioMinimumVersion);
        await Assert.That(solutionFile.ReadContent()).IsEqualTo(expectedContent);
    }

    [Test]
    public async Task UpdateFileHeader_WithFullV15Header()
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
        await Assert.That(fileHeader.FileFormatVersion).IsEqualTo(SolutionFileHeader.SupportedFileFormatVersion);
        await Assert.That(fileHeader.LastVisualStudioMajorVersion).IsEqualTo(expectedVersion.Major);
        await Assert.That(fileHeader.LastVisualStudioVersion).IsEqualTo(expectedVersion);
        await Assert.That(fileHeader.MinimumVisualStudioVersion).IsEqualTo(SolutionFileBuilder.DefaultVisualStudioMinimumVersion);
        await Assert.That(solutionFile.ReadContent()).IsEqualTo(expectedContent);
    }

    [Test]
    public async Task UpdateFileHeader_WithMinimalHeader()
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
        await Assert.That(fileHeader.FileFormatVersion).IsEqualTo(SolutionFileHeader.SupportedFileFormatVersion);
        await Assert.That(fileHeader.LastVisualStudioMajorVersion).IsEqualTo(expectedVersion.Major);
        await Assert.That(fileHeader.LastVisualStudioVersion).IsEqualTo(expectedVersion);
        await Assert.That(fileHeader.MinimumVisualStudioVersion).IsEqualTo(Version.Parse(SolutionFileHeader.DefaultMinimumVisualStudioVersion));
        await Assert.That(solutionFile.ReadContent()).IsEqualTo(expectedFileContent);
    }

    [Test]
    public async Task UpdateFileHeader_WithMissingMajorVersion()
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
        await Assert.That(fileHeader.FileFormatVersion).IsEqualTo(SolutionFileHeader.SupportedFileFormatVersion);
        await Assert.That(fileHeader.LastVisualStudioMajorVersion).IsEqualTo(expectedVersion.Major);
        await Assert.That(fileHeader.LastVisualStudioVersion).IsEqualTo(expectedVersion);
        await Assert.That(fileHeader.MinimumVisualStudioVersion).IsEqualTo(Version.Parse(SolutionFileHeader.DefaultMinimumVisualStudioVersion));
        await Assert.That(solutionFile.ReadContent()).IsEqualTo(expectedFileContent);
    }

    [Test]
    public async Task UpdateFileHeader_WithMissingMinimumVersion()
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
        await Assert.That(fileHeader.FileFormatVersion).IsEqualTo(SolutionFileHeader.SupportedFileFormatVersion);
        await Assert.That(fileHeader.LastVisualStudioMajorVersion).IsEqualTo(expectedVersion.Major);
        await Assert.That(fileHeader.LastVisualStudioVersion).IsEqualTo(expectedVersion);
        await Assert.That(fileHeader.MinimumVisualStudioVersion).IsEqualTo(Version.Parse(SolutionFileHeader.DefaultMinimumVisualStudioVersion));
        await Assert.That(solutionFile.ReadContent()).IsEqualTo(expectedFileContent);
    }

    [Test]
    public async Task UpdateFileHeader_WithMissingVersion()
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
        await Assert.That(fileHeader.FileFormatVersion).IsEqualTo(SolutionFileHeader.SupportedFileFormatVersion);
        await Assert.That(fileHeader.LastVisualStudioMajorVersion).IsEqualTo(expectedVersion.Major);
        await Assert.That(fileHeader.LastVisualStudioVersion).IsEqualTo(expectedVersion);
        await Assert.That(fileHeader.MinimumVisualStudioVersion).IsEqualTo(Version.Parse(SolutionFileHeader.DefaultMinimumVisualStudioVersion));
        await Assert.That(solutionFile.ReadContent()).IsEqualTo(expectedFileContent);
    }

    [Test]
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

    [Test]
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
