namespace SlnUp.Json.Tests.Extensions;

using SlnUp.Json.Extensions;

public class AssemblyExtensionsTests
{
    [Fact]
    public void GetEmbeddedFileResourceContent()
    {
        // Arrange
        string resourceName = "TestVersions.json";

        // Act
        string content = AssemblyExtensions.GetEmbeddedFileResourceContent(
            typeof(AssemblyExtensionsTests).Assembly,
            resourceName);

        // Assert
        Assert.NotNull(content);
    }

    [Fact]
    public void GetEmbeddedFileResourceContent_ResourceNotFound()
    {
        // Arrange, act, and assert
        Assert.Throws<ArgumentException>(() => AssemblyExtensions.GetEmbeddedFileResourceContent(
            typeof(AssemblyExtensionsTests).Assembly,
            "nothing"));
    }
}
