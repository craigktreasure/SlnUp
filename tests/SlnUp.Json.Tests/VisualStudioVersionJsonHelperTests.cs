namespace SlnUp.Json.Tests;

using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;

using SlnUp.Core;
using SlnUp.TestLibrary.Extensions;

public class VisualStudioVersionJsonHelperTests
{
    private const string commonVersionsJson = /*lang=json,strict*/ @"[
  {
    ""BuildVersion"": ""16.11.31829.152"",
    ""Channel"": ""Release"",
    ""IsPreview"": false,
    ""Product"": ""visualStudio2019"",
    ""Version"": ""16.11.6""
  },
  {
    ""BuildVersion"": ""17.0.31903.59"",
    ""Channel"": ""Release"",
    ""IsPreview"": false,
    ""Product"": ""visualStudio2022"",
    ""Version"": ""17.0.0""
  }
]";

    private static readonly IReadOnlyList<VisualStudioVersion> commonVersions =
    [
        new VisualStudioVersion(
            VisualStudioProduct.VisualStudio2019,
            Version.Parse("16.11.6"),
            Version.Parse("16.11.31829.152"),
            "Release"),
        new VisualStudioVersion(
            VisualStudioProduct.VisualStudio2022,
            Version.Parse("17.0.0"),
            Version.Parse("17.0.31903.59"),
            "Release"),
    ];

    [Fact]
    public void FromJson()
    {
        // Act
        IReadOnlyList<VisualStudioVersion> versions = VisualStudioVersionJsonHelper.FromJson(commonVersionsJson);

        // Assert
        Assert.Equal(commonVersions, versions);
    }

    [Fact]
    public void FromJson_Empty()
    {
        // Act
        IReadOnlyList<VisualStudioVersion> versions = VisualStudioVersionJsonHelper.FromJson("[]");

        // Assert
        Assert.Empty(versions);
    }

    [Fact]
    public void FromJson_EmptyString()
    {
        // Act
        Assert.Throws<JsonException>(() => VisualStudioVersionJsonHelper.FromJson(string.Empty));
    }

    [Fact]
    public void FromJson_Null()
    {
        // Act
        Assert.Throws<InvalidDataException>(() => VisualStudioVersionJsonHelper.FromJson("null"));
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
        IReadOnlyList<VisualStudioVersion> versions = VisualStudioVersionJsonHelper.FromJsonFile(fileSystem, filePath);

        // Assert
        Assert.Equal(commonVersions, versions);
    }

    [Fact]
    public void ToJson()
    {
        // Act
        string json = VisualStudioVersionJsonHelper.ToJson(commonVersions);

        // Assert
        Assert.Equal(commonVersionsJson, json);
    }

    [Fact]
    public void ToJson_Empty()
    {
        // Arrange
        const string expectedJson = "[]";

        // Act
        string json = VisualStudioVersionJsonHelper.ToJson([]);

        // Assert
        Assert.Equal(expectedJson, json);
    }

    [Fact]
    public void ToJson_Null()
    {
        // Arrange
        const string expectedJson = "null";

        // Act
        string json = VisualStudioVersionJsonHelper.ToJson(null!);

        // Assert
        Assert.Equal(expectedJson, json);
    }

    [Fact]
    public void ToJsonFile()
    {
        // Arrange
        MockFileSystem fileSystem = new();
        string filePath = "C:/foo.json".ToCrossPlatformPath();

        // Act
        VisualStudioVersionJsonHelper.ToJsonFile(fileSystem, commonVersions, filePath);

        // Assert
        Assert.True(fileSystem.FileExists(filePath));
        Assert.Equal(commonVersionsJson, fileSystem.GetFile(filePath).TextContents);
    }
}
