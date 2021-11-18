namespace SlnUp.Core.Tests;

using FluentAssertions;
using SlnUp.TestLibrary;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using Xunit;

public class VisualStudioVersionTests
{
    private const string commonVersionsJson = @"[
  {
    ""Product"": ""visualStudio2019"",
    ""Version"": ""16.11.6"",
    ""BuildVersion"": ""16.11.31829.152"",
    ""Channel"": ""Release"",
    ""IsPreview"": false
  },
  {
    ""Product"": ""visualStudio2022"",
    ""Version"": ""17.0.0"",
    ""BuildVersion"": ""17.0.31903.59"",
    ""Channel"": ""Release"",
    ""IsPreview"": false
  }
]";

    private static readonly IReadOnlyList<VisualStudioVersion> commonVersions = new[]
    {
        new VisualStudioVersion(
            VisualStudioProduct.VisualStudio2019,
            Version.Parse("16.11.6"),
            Version.Parse("16.11.31829.152"),
            "Release"),
        new VisualStudioVersion(
            VisualStudioProduct.VisualStudio2022,
            Version.Parse("17.0.0"),
            Version.Parse("17.0.31903.59"),
            "Release")
    };

    [Fact]
    public void Constructor()
    {
        // Arrange
        const string expectedChannel = "Release";
        const string expectedFullVersionTitle = "Visual Studio 2022 17.0";
        const string expectedVersionTitle = "Visual Studio 2022";
        const string expectedVersion = "17.0.0";
        const string expectedBuildVersion = "17.0.31903.59";

        // Act
        VisualStudioVersion version = new(
            VisualStudioProduct.VisualStudio2022,
            Version.Parse(expectedVersion),
            Version.Parse(expectedBuildVersion),
            expectedChannel);

        // Assert
        version.Channel.Should().Be(expectedChannel);
        version.Version.ToString().Should().Be(expectedVersion);
        version.BuildVersion.ToString().Should().Be(expectedBuildVersion);
        version.ProductTitle.Should().Be(expectedVersionTitle);
        version.FullProductTitle.Should().Be(expectedFullVersionTitle);
    }

    [Fact]
    public void ConstructorWithPreview()
    {
        // Arrange
        const string expectedChannel = "Preview 1";
        const string expectedFullVersionTitle = "Visual Studio 2022 17.0.1 Preview 1";
        const string expectedVersionTitle = "Visual Studio 2022";
        const string expectedVersion = "17.0.1";
        const string expectedBuildVersion = "17.1.31903.286";

        // Act
        VisualStudioVersion version = new(
            VisualStudioProduct.VisualStudio2022,
            Version.Parse(expectedVersion),
            Version.Parse(expectedBuildVersion),
            expectedChannel,
            IsPreview: true);

        // Assert
        version.Channel.Should().Be(expectedChannel);
        version.Version.ToString().Should().Be(expectedVersion);
        version.BuildVersion.ToString().Should().Be(expectedBuildVersion);
        version.ProductTitle.Should().Be(expectedVersionTitle);
        version.FullProductTitle.Should().Be(expectedFullVersionTitle);
    }

    [Fact]
    public void FromJson()
    {
        // Act
        IReadOnlyList<VisualStudioVersion> versions = VisualStudioVersion.FromJson(commonVersionsJson);

        // Assert
        versions.Should().BeEquivalentTo(commonVersions);
    }

    [Fact]
    public void FromJsonEmpty()
    {
        // Act
        IReadOnlyList<VisualStudioVersion> versions = VisualStudioVersion.FromJson("[]");

        // Assert
        versions.Should().BeEmpty();
    }

    [Fact]
    public void FromJsonEmptyString()
    {
        // Act
        Assert.Throws<JsonException>(() => VisualStudioVersion.FromJson(string.Empty));
    }

    [Fact]
    public void FromJsonFile()
    {
        // Arrange
        string filePath = "C:/foo.json".ToCrossPlatformPath();
        IFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            [filePath] = new MockFileData(commonVersionsJson),
        });

        // Act
        IReadOnlyList<VisualStudioVersion> versions = VisualStudioVersion.FromJsonFile(fileSystem, filePath);

        // Assert
        versions.Should().BeEquivalentTo(commonVersions);
    }

    [Fact]
    public void FromJsonNull()
    {
        // Act
        Assert.Throws<InvalidDataException>(() => VisualStudioVersion.FromJson("null"));
    }

    [Fact]
    public void GetHashCodeMethod()
    {
        // Arrange
        VisualStudioVersion version = commonVersions[0];
        int expectedHashCode = version.BuildVersion.GetHashCode();

        // Act
        int hashCode = version.GetHashCode();

        // Assert
        hashCode.Should().Be(expectedHashCode);
    }

    [Fact]
    public void ToJson()
    {
        // Act
        string json = VisualStudioVersion.ToJson(commonVersions);

        // Assert
        json.Should().Be(commonVersionsJson);
    }

    [Fact]
    public void ToJsonEmpty()
    {
        // Arrange
        const string expectedJson = "[]";

        // Act
        string json = VisualStudioVersion.ToJson(Enumerable.Empty<VisualStudioVersion>());

        // Assert
        json.Should().Be(expectedJson);
    }

    [Fact]
    public void ToJsonFile()
    {
        // Arrange
        MockFileSystem fileSystem = new();
        string filePath = "C:/foo.json".ToCrossPlatformPath();

        // Act
        VisualStudioVersion.ToJsonFile(fileSystem, commonVersions, filePath);

        // Assert
        fileSystem.FileExists(filePath).Should().BeTrue();
        fileSystem.GetFile(filePath).TextContents.Should().Be(commonVersionsJson);
    }

    [Fact]
    public void ToJsonNull()
    {
        // Arrange
        const string expectedJson = "null";

        // Act
        string json = VisualStudioVersion.ToJson(null!);

        // Assert
        json.Should().Be(expectedJson);
    }
}
