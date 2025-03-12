namespace SlnUp.Core.Tests.Extensions;

using SlnUp.Core.Extensions;

public class VersionExtensionsTests
{
    [Test]
    [Arguments("0.0", "0.0", true)]
    [Arguments("0.0", "0.1", false)]
    [Arguments("0.0.0", "0.0.0", true)]
    [Arguments("0.0.0", "0.1.0", false)]
    [Arguments("0.0.0", "0.0.1", true)]
    [Arguments("0.0.0.0", "0.0.0.0", true)]
    [Arguments("0.0.0.0", "0.1.0.0", false)]
    [Arguments("0.0.0.0", "0.0.0.1", true)]
    public async Task HasSameMajorMinor(string versionAInput, string versionBInput, bool expectedResult)
    {
        // Arrange
        Version versionA = Version.Parse(versionAInput);
        Version versionB = Version.Parse(versionBInput);

        // Act
        bool result = versionA.HasSameMajorMinor(versionB);

        // Assert
        await Assert.That(result).IsEqualTo(expectedResult);
    }

    [Test]
    [Arguments("0.0", true)]
    [Arguments("0.0.0", false)]
    [Arguments("0.0.0.0", false)]
    public async Task Is2PartVersion(string versionInput, bool expectedResult)
    {
        // Arrange
        Version version = Version.Parse(versionInput);

        // Act
        bool result = version.Is2PartVersion();

        // Assert
        await Assert.That(result).IsEqualTo(expectedResult);
    }

    [Test]
    [Arguments("0.0", false)]
    [Arguments("0.0.0", true)]
    [Arguments("0.0.0.0", false)]
    public async Task Is3PartVersion(string versionInput, bool expectedResult)
    {
        // Arrange
        Version version = Version.Parse(versionInput);

        // Act
        bool result = version.Is3PartVersion();

        // Assert
        await Assert.That(result).IsEqualTo(expectedResult);
    }

    [Test]
    [Arguments("0.0", false)]
    [Arguments("0.0.0", false)]
    [Arguments("0.0.0.0", true)]
    public async Task Is4PartVersion(string versionInput, bool expectedResult)
    {
        // Arrange
        Version version = Version.Parse(versionInput);

        // Act
        bool result = version.Is4PartVersion();

        // Assert
        await Assert.That(result).IsEqualTo(expectedResult);
    }
}
