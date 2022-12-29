namespace SlnUp.Core.Tests.Extensions;

using SlnUp.Core.Extensions;

public class VersionExtensionsTests
{
    [Theory]
    [InlineData("0.0", "0.0", true)]
    [InlineData("0.0", "0.1", false)]
    [InlineData("0.0.0", "0.0.0", true)]
    [InlineData("0.0.0", "0.1.0", false)]
    [InlineData("0.0.0", "0.0.1", true)]
    [InlineData("0.0.0.0", "0.0.0.0", true)]
    [InlineData("0.0.0.0", "0.1.0.0", false)]
    [InlineData("0.0.0.0", "0.0.0.1", true)]
    public void HasSameMajorMinor(string versionAInput, string versionBInput, bool expectedResult)
    {
        // Arrange
        Version versionA = Version.Parse(versionAInput);
        Version versionB = Version.Parse(versionBInput);

        // Act
        bool result = versionA.HasSameMajorMinor(versionB);

        // Assert
        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("0.0", true)]
    [InlineData("0.0.0", false)]
    [InlineData("0.0.0.0", false)]
    public void Is2PartVersion(string versionInput, bool expectedResult)
    {
        // Arrange
        Version version = Version.Parse(versionInput);

        // Act
        bool result = version.Is2PartVersion();

        // Assert
        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("0.0", false)]
    [InlineData("0.0.0", true)]
    [InlineData("0.0.0.0", false)]
    public void Is3PartVersion(string versionInput, bool expectedResult)
    {
        // Arrange
        Version version = Version.Parse(versionInput);

        // Act
        bool result = version.Is3PartVersion();

        // Assert
        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("0.0", false)]
    [InlineData("0.0.0", false)]
    [InlineData("0.0.0.0", true)]
    public void Is4PartVersion(string versionInput, bool expectedResult)
    {
        // Arrange
        Version version = Version.Parse(versionInput);

        // Act
        bool result = version.Is4PartVersion();

        // Assert
        result.Should().Be(expectedResult);
    }
}
