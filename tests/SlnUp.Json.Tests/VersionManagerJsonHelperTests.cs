namespace SlnUp.Json.Tests;

using SlnUp.Core;

public class VersionManagerJsonHelperTests
{
    [Test]
    public async Task LoadFromEmbeddedResource()
    {
        // Arrange
        string resourceName = "TestVersions.json";

        // Act
        VersionManager versionManager = VersionManagerJsonHelper.LoadFromEmbeddedResource(
            typeof(VersionManagerJsonHelperTests).Assembly,
            resourceName);

        // Assert
        await Assert.That(versionManager).IsNotNull();
    }

    [Test]
    public void LoadFromEmbeddedResource_ResourceNotFound()
    {
        // Arrange, act, and assert
        Assert.Throws<ArgumentException>(() => VersionManagerJsonHelper.LoadFromEmbeddedResource(
            typeof(VersionManagerJsonHelperTests).Assembly,
            "nothing"));
    }
}
