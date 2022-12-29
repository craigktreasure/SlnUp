namespace SlnUp.Json.Tests;

using SlnUp.Core;

public class VersionManagerJsonHelperTests
{
    [Fact]
    public void LoadFromEmbeddedResource()
    {
        // Arrange
        string resourceName = "TestVersions.json";

        // Act
        VersionManager versionManager = VersionManagerJsonHelper.LoadFromEmbeddedResource(
            typeof(VersionManagerJsonHelperTests).Assembly,
            resourceName);

        // Assert
        Assert.NotNull(versionManager);
    }

    [Fact]
    public void LoadFromEmbeddedResource_ResourceNotFound()
    {
        // Arrange, act, and assert
        Assert.Throws<ArgumentException>(() => VersionManagerJsonHelper.LoadFromEmbeddedResource(
            typeof(VersionManagerJsonHelperTests).Assembly,
            "nothing"));
    }
}
