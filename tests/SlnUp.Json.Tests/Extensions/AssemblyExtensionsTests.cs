namespace SlnUp.Json.Tests.Extensions;

using SlnUp.Json.Extensions;

public class AssemblyExtensionsTests
{
    [Test]
    public async Task GetEmbeddedFileResourceContent()
    {
        // Arrange
        string resourceName = "TestVersions.json";

        // Act
        string content = AssemblyExtensions.GetEmbeddedFileResourceContent(
            typeof(AssemblyExtensionsTests).Assembly,
            resourceName);

        // Assert
        await Assert.That(content).IsNotNull();
    }

    [Test]
    public void GetEmbeddedFileResourceContent_ResourceNotFound()
    {
        // Arrange, act, and assert
        Assert.Throws<ArgumentException>(() => AssemblyExtensions.GetEmbeddedFileResourceContent(
            typeof(AssemblyExtensionsTests).Assembly,
            "nothing"));
    }
}
