namespace SlnUp.Core.Tests;

using FluentAssertions;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using Xunit;

public class VisualStudioVersionDetailTests
{
    private const string commonVersionsJson = @"[
  {
    ""VisualStudioVersion"": ""visualStudio2019"",
    ""Version"": ""16.11.6"",
    ""BuildVersion"": ""16.11.31829.152"",
    ""Channel"": ""Release"",
    ""IsPreview"": false
  },
  {
    ""VisualStudioVersion"": ""visualStudio2022"",
    ""Version"": ""17.0.0"",
    ""BuildVersion"": ""17.0.31903.59"",
    ""Channel"": ""Release"",
    ""IsPreview"": false
  }
]";

    private static readonly IReadOnlyList<VisualStudioVersionDetail> commonVersions = new[]
    {
        new VisualStudioVersionDetail(
            VisualStudioVersion.VisualStudio2019,
            Version.Parse("16.11.6"),
            Version.Parse("16.11.31829.152"),
            "Release"),
        new VisualStudioVersionDetail(
            VisualStudioVersion.VisualStudio2022,
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
        VisualStudioVersionDetail version = new(
            VisualStudioVersion.VisualStudio2022,
            Version.Parse(expectedVersion),
            Version.Parse(expectedBuildVersion),
            expectedChannel);

        // Assert
        version.Channel.Should().Be(expectedChannel);
        version.Version.ToString().Should().Be(expectedVersion);
        version.BuildVersion.ToString().Should().Be(expectedBuildVersion);
        version.VisualStudioTitle.Should().Be(expectedVersionTitle);
        version.VisualStudioFullTitle.Should().Be(expectedFullVersionTitle);
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
        VisualStudioVersionDetail version = new(
            VisualStudioVersion.VisualStudio2022,
            Version.Parse(expectedVersion),
            Version.Parse(expectedBuildVersion),
            expectedChannel,
            IsPreview: true);

        // Assert
        version.Channel.Should().Be(expectedChannel);
        version.Version.ToString().Should().Be(expectedVersion);
        version.BuildVersion.ToString().Should().Be(expectedBuildVersion);
        version.VisualStudioTitle.Should().Be(expectedVersionTitle);
        version.VisualStudioFullTitle.Should().Be(expectedFullVersionTitle);
    }

    [Fact]
    public void FromJson()
    {
        // Act
        IReadOnlyList<VisualStudioVersionDetail> versions = VisualStudioVersionDetail.FromJson(commonVersionsJson);

        // Assert
        versions.Should().BeEquivalentTo(commonVersions);
    }

    [Fact]
    public void FromJsonEmpty()
    {
        // Act
        IReadOnlyList<VisualStudioVersionDetail> versions = VisualStudioVersionDetail.FromJson("[]");

        // Assert
        versions.Should().BeEmpty();
    }

    [Fact]
    public void FromJsonEmptyString()
    {
        // Act
        Assert.Throws<JsonException>(() => VisualStudioVersionDetail.FromJson(string.Empty));
    }

    [Fact]
    public void FromJsonFile()
    {
        // Arrange
        const string filePath = "C:/foo.json";
        IFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            [filePath] = new MockFileData(commonVersionsJson),
        });

        // Act
        IReadOnlyList<VisualStudioVersionDetail> versions = VisualStudioVersionDetail.FromJsonFile(fileSystem, filePath);

        // Assert
        versions.Should().BeEquivalentTo(commonVersions);
    }

    [Fact]
    public void FromJsonNull()
    {
        // Act
        Assert.Throws<InvalidDataException>(() => VisualStudioVersionDetail.FromJson("null"));
    }

    [Fact]
    public void ToJson()
    {
        // Act
        string json = VisualStudioVersionDetail.ToJson(commonVersions);

        // Assert
        json.Should().Be(commonVersionsJson);
    }

    [Fact]
    public void ToJsonEmpty()
    {
        // Arrange
        const string expectedJson = "[]";

        // Act
        string json = VisualStudioVersionDetail.ToJson(Enumerable.Empty<VisualStudioVersionDetail>());

        // Assert
        json.Should().Be(expectedJson);
    }

    [Fact]
    public void ToJsonFile()
    {
        // Arrange
        MockFileSystem fileSystem = new();
        const string filePath = "C:/foo.json";

        // Act
        VisualStudioVersionDetail.ToJsonFile(fileSystem, commonVersions, filePath);

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
        string json = VisualStudioVersionDetail.ToJson(null!);

        // Assert
        json.Should().Be(expectedJson);
    }
}
