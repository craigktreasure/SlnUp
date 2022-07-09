namespace SlnUp.Tests.Extensions;

public class AssemblyExtensionsTests
{
    [Fact]
    public void GetEmbeddedFileResourceContentResourceNotFound()
    {
        // Arrange, act, and assert
        Assert.Throws<ArgumentException>(() => AssemblyExtensions.GetEmbeddedFileResourceContent(
            typeof(AssemblyExtensionsTests).Assembly,
            "nothing"));
    }
}
