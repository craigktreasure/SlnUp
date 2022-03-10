namespace SlnUp.Tests;

using FluentAssertions;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

[SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "Not necessary in this case.")]
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

        // Act
        Assert.Throws<InvalidDataException>(() => new SolutionFile(fileSystem, filePath));
    }

    [Fact]
    public void ConstructWithFullHeader()
    {
        // Arrange
        const string filePath = "C:\\MyProject.sln";
        MockFileSystem fileSystem = new(new Dictionary<string, MockFileData>
        {
            [filePath] = new MockFileData(@"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 16
VisualStudioVersion = 16.0.30114.105
MinimumVisualStudioVersion = 10.0.40219.1
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
EndGlobal
")
        });

        // Act
        SolutionFile solutionFile = new(fileSystem, filePath);

        // Assert
        SolutionFileHeader fileHeader = solutionFile.FileHeader;
        fileHeader.FileFormatVersion.Should().Be("12.00");
        fileHeader.LastVisualStudioMajorVersion.Should().Be(16);
        fileHeader.LastVisualStudioVersion.Should().Be(Version.Parse("16.0.30114.105"));
        fileHeader.MinimumVisualStudioVersion.Should().Be(Version.Parse("10.0.40219.1"));
    }

    [Fact]
    public void ConstructWithFullV15Header()
    {
        // Arrange
        const string filePath = "C:\\MyProject.sln";
        MockFileSystem fileSystem = new(new Dictionary<string, MockFileData>
        {
            [filePath] = new MockFileData(@"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio 15
VisualStudioVersion = 15.0.26124.0
MinimumVisualStudioVersion = 15.0.26124.0
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
EndGlobal
")
        });

        // Act
        SolutionFile solutionFile = new(fileSystem, filePath);

        // Assert
        SolutionFileHeader fileHeader = solutionFile.FileHeader;
        fileHeader.FileFormatVersion.Should().Be("12.00");
        fileHeader.LastVisualStudioMajorVersion.Should().Be(15);
        fileHeader.LastVisualStudioVersion.Should().Be(Version.Parse("15.0.26124.0"));
        fileHeader.MinimumVisualStudioVersion.Should().Be(Version.Parse("15.0.26124.0"));
    }

    [Fact]
    public void ConstructWithMinimalHeader()
    {
        // Arrange
        const string filePath = "C:\\MyProject.sln";
        MockFileSystem fileSystem = new(new Dictionary<string, MockFileData>
        {
            [filePath] = new MockFileData(@"
Microsoft Visual Studio Solution File, Format Version 12.00
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
EndGlobal
")
        });

        // Act
        SolutionFile solutionFile = new(fileSystem, filePath);

        // Assert
        SolutionFileHeader fileHeader = solutionFile.FileHeader;
        fileHeader.FileFormatVersion.Should().Be("12.00");
        fileHeader.LastVisualStudioMajorVersion.Should().BeNull();
        fileHeader.LastVisualStudioVersion.Should().BeNull();
        fileHeader.MinimumVisualStudioVersion.Should().BeNull();
    }

    [Fact]
    public void ConstructWithMissingFileFormatVersion()
    {
        // Arrange
        const string filePath = "C:\\MyProject.sln";
        MockFileSystem fileSystem = new(new Dictionary<string, MockFileData>
        {
            [filePath] = new MockFileData(@"
# Visual Studio Version 16
VisualStudioVersion = 16.0.30114.105
MinimumVisualStudioVersion = 10.0.40219.1
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
EndGlobal
")
        });

        // Act
        Assert.Throws<InvalidDataException>(() => new SolutionFile(fileSystem, filePath));
    }

    [Fact]
    public void UpdateFileHeaderWithFullHeader()
    {
        // Arrange
        const string filePath = "C:\\MyProject.sln";
        const string fileFormat = @"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version {0}
VisualStudioVersion = {1}
MinimumVisualStudioVersion = 10.0.40219.1
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
EndGlobal
";
        MockFileSystem fileSystem = new(new Dictionary<string, MockFileData>
        {
            [filePath] = new MockFileData(string.Format(fileFormat, 16, "16.0.30114.105"))
        });
        SolutionFile solutionFile = new(fileSystem, filePath);
        Version expectedVersion = Version.Parse("17.0.31903.59");

        // Act
        solutionFile.UpdateFileHeader(expectedVersion);

        // Assert
        SolutionFileHeader fileHeader = solutionFile.FileHeader;
        fileHeader.FileFormatVersion.Should().Be("12.00");
        fileHeader.LastVisualStudioMajorVersion.Should().Be(expectedVersion.Major);
        fileHeader.LastVisualStudioVersion.Should().Be(expectedVersion);
        fileHeader.MinimumVisualStudioVersion.Should().Be(Version.Parse("10.0.40219.1"));
        string fileContent = fileSystem.File.ReadAllText(filePath);
        fileContent.Should().Be(string.Format(fileFormat, expectedVersion.Major, expectedVersion));
    }

    [Fact]
    public void UpdateFileHeaderWithFullV15Header()
    {
        // Arrange
        const string filePath = "C:\\MyProject.sln";
        const string fileFormat = @"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio {0}
VisualStudioVersion = {1}
MinimumVisualStudioVersion = 10.0.40219.1
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
EndGlobal
";
        MockFileSystem fileSystem = new(new Dictionary<string, MockFileData>
        {
            [filePath] = new MockFileData(string.Format(fileFormat, 15, "15.0.26124.0"))
        });
        SolutionFile solutionFile = new(fileSystem, filePath);
        Version expectedVersion = Version.Parse("15.0.27000.0");

        // Act
        solutionFile.UpdateFileHeader(expectedVersion);

        // Assert
        SolutionFileHeader fileHeader = solutionFile.FileHeader;
        fileHeader.FileFormatVersion.Should().Be("12.00");
        fileHeader.LastVisualStudioMajorVersion.Should().Be(expectedVersion.Major);
        fileHeader.LastVisualStudioVersion.Should().Be(expectedVersion);
        fileHeader.MinimumVisualStudioVersion.Should().Be(Version.Parse("10.0.40219.1"));
        string fileContent = fileSystem.File.ReadAllText(filePath);
        fileContent.Should().Be(string.Format(fileFormat, expectedVersion.Major, expectedVersion));
    }

    [Fact]
    public void UpdateFileHeaderWithMinimalHeader()
    {
        // Arrange
        const string filePath = "C:\\MyProject.sln";
        Version expectedVersion = Version.Parse("17.0.31903.59");
        string expectedFileContent = $@"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version {expectedVersion.Major}
VisualStudioVersion = {expectedVersion}
MinimumVisualStudioVersion = 10.0.40219.1
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
EndGlobal
";
        MockFileSystem fileSystem = new(new Dictionary<string, MockFileData>
        {
            [filePath] = new MockFileData(@"
Microsoft Visual Studio Solution File, Format Version 12.00
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
EndGlobal
")
        });
        SolutionFile solutionFile = new(fileSystem, filePath);

        // Act
        solutionFile.UpdateFileHeader(expectedVersion);

        // Assert
        SolutionFileHeader fileHeader = solutionFile.FileHeader;
        fileHeader.FileFormatVersion.Should().Be(SolutionFileHeader.SupportedFileFormatVersion);
        fileHeader.LastVisualStudioMajorVersion.Should().Be(expectedVersion.Major);
        fileHeader.LastVisualStudioVersion.Should().Be(expectedVersion);
        fileHeader.MinimumVisualStudioVersion.Should().Be(Version.Parse(SolutionFileHeader.DefaultMinimumVisualStudioVersion));
        string fileContent = fileSystem.File.ReadAllText(filePath);
        fileContent.Should().Be(expectedFileContent);
    }

    [Fact]
    public void UpdateFileHeaderWithMissingMajorVersion()
    {
        // Arrange
        const string filePath = "C:\\MyProject.sln";
        Version expectedVersion = Version.Parse("17.0.31903.59");
        string expectedFileContent = $@"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version {expectedVersion.Major}
VisualStudioVersion = {expectedVersion}
MinimumVisualStudioVersion = 10.0.40219.1
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
EndGlobal
";
        MockFileSystem fileSystem = new(new Dictionary<string, MockFileData>
        {
            [filePath] = new MockFileData($@"
Microsoft Visual Studio Solution File, Format Version 12.00
VisualStudioVersion = {expectedVersion}
MinimumVisualStudioVersion = 10.0.40219.1
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
EndGlobal
")
        });
        SolutionFile solutionFile = new(fileSystem, filePath);

        // Act
        solutionFile.UpdateFileHeader(expectedVersion);

        // Assert
        SolutionFileHeader fileHeader = solutionFile.FileHeader;
        fileHeader.FileFormatVersion.Should().Be(SolutionFileHeader.SupportedFileFormatVersion);
        fileHeader.LastVisualStudioMajorVersion.Should().Be(expectedVersion.Major);
        fileHeader.LastVisualStudioVersion.Should().Be(expectedVersion);
        fileHeader.MinimumVisualStudioVersion.Should().Be(Version.Parse(SolutionFileHeader.DefaultMinimumVisualStudioVersion));
        string fileContent = fileSystem.File.ReadAllText(filePath);
        fileContent.Should().Be(expectedFileContent);
    }

    [Fact]
    public void UpdateFileHeaderWithMissingMinimumVersion()
    {
        // Arrange
        const string filePath = "C:\\MyProject.sln";
        Version expectedVersion = Version.Parse("17.0.31903.59");
        string expectedFileContent = $@"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version {expectedVersion.Major}
VisualStudioVersion = {expectedVersion}
MinimumVisualStudioVersion = 10.0.40219.1
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
EndGlobal
";
        MockFileSystem fileSystem = new(new Dictionary<string, MockFileData>
        {
            [filePath] = new MockFileData($@"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version {expectedVersion.Major}
VisualStudioVersion = {expectedVersion}
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
EndGlobal
")
        });
        SolutionFile solutionFile = new(fileSystem, filePath);

        // Act
        solutionFile.UpdateFileHeader(expectedVersion);

        // Assert
        SolutionFileHeader fileHeader = solutionFile.FileHeader;
        fileHeader.FileFormatVersion.Should().Be(SolutionFileHeader.SupportedFileFormatVersion);
        fileHeader.LastVisualStudioMajorVersion.Should().Be(expectedVersion.Major);
        fileHeader.LastVisualStudioVersion.Should().Be(expectedVersion);
        fileHeader.MinimumVisualStudioVersion.Should().Be(Version.Parse(SolutionFileHeader.DefaultMinimumVisualStudioVersion));
        string fileContent = fileSystem.File.ReadAllText(filePath);
        fileContent.Should().Be(expectedFileContent);
    }

    [Fact]
    public void UpdateFileHeaderWithMissingVersion()
    {
        // Arrange
        const string filePath = "C:\\MyProject.sln";
        Version expectedVersion = Version.Parse("17.0.31903.59");
        string expectedFileContent = $@"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version {expectedVersion.Major}
VisualStudioVersion = {expectedVersion}
MinimumVisualStudioVersion = 10.0.40219.1
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
EndGlobal
";
        MockFileSystem fileSystem = new(new Dictionary<string, MockFileData>
        {
            [filePath] = new MockFileData($@"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version {expectedVersion.Major}
MinimumVisualStudioVersion = 10.0.40219.1
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
EndGlobal
")
        });
        SolutionFile solutionFile = new(fileSystem, filePath);

        // Act
        solutionFile.UpdateFileHeader(expectedVersion);

        // Assert
        SolutionFileHeader fileHeader = solutionFile.FileHeader;
        fileHeader.FileFormatVersion.Should().Be(SolutionFileHeader.SupportedFileFormatVersion);
        fileHeader.LastVisualStudioMajorVersion.Should().Be(expectedVersion.Major);
        fileHeader.LastVisualStudioVersion.Should().Be(expectedVersion);
        fileHeader.MinimumVisualStudioVersion.Should().Be(Version.Parse(SolutionFileHeader.DefaultMinimumVisualStudioVersion));
        string fileContent = fileSystem.File.ReadAllText(filePath);
        fileContent.Should().Be(expectedFileContent);
    }
}
