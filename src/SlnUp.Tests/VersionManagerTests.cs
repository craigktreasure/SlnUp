namespace SlnUp.Tests;

using FluentAssertions;
using SlnUp.Core;
using Xunit;

public class VersionManagerTests
{
    [Theory]
    [InlineData(null, false, null)]
    [InlineData("", false, null)]
    [InlineData(" ", false, null)]
    [InlineData("0", false, null)]
    [InlineData("0.0", false, null)]
    [InlineData("0.0.0", false, null)]
    [InlineData("0.0.0.0", false, null)]
    [InlineData("15.2", true, "15.0.26430.16")]
    [InlineData("15.2.5", true, "15.0.26430.15")]
    [InlineData("15.99", false, null)]
    [InlineData("15.2.99", false, null)]
    [InlineData("2017", true, "15.9.28307.1778")]
    [InlineData("16.9", true, "16.9.32106.192")]
    [InlineData("16.7.21", true, "16.7.31828.227")]
    [InlineData("16.99", false, null)]
    [InlineData("16.7.99", false, null)]
    [InlineData("2019", true, "16.11.32106.194")]
    [InlineData("17.0", true, "17.0.32112.339")]
    [InlineData("17.0.0", true, "17.0.31903.59")]
    [InlineData("17.99", false, null)]
    [InlineData("17.0.99", false, null)]
    [InlineData("2022", true, "17.0.32112.339")]
    public void FromVersionParameter(string? input, bool expectFound, string? expectedBuildVersion)
    {
        // Arrange
        if (expectFound && expectedBuildVersion is null)
        {
            throw new ArgumentNullException(nameof(expectedBuildVersion), "We expect a value to be found but an expected build version wasn't provided.");
        }

        VersionManager versionManager = VersionManager.LoadFromEmbeddedResource();

        // Act
        VisualStudioVersion? version = versionManager.FromVersionParameter(input);

        // Assert
        if (!expectFound)
        {
            version.Should().BeNull();
        }
        else
        {
            version.Should().NotBeNull();
            version!.BuildVersion.Should().Be(Version.Parse(expectedBuildVersion!));
        }
    }

    [Fact]
    public void LoadFromEmbeddedResource()
    {
        // Arrange, act, and assert
        VersionManager.LoadFromEmbeddedResource();
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData(" ", false)]
    [InlineData("0", false)]
    [InlineData("0.0", true)]
    [InlineData("0.0.0", true)]
    [InlineData("0.0.0.0", false)]
    [InlineData("2022", false)]
    public void TryParseVisualStudioVersion(string? input, bool expectedResult)
    {
        // Act
        bool result = VersionManager.TryParseVisualStudioVersion(input, out Version? version);

        // Assert
        result.Should().Be(expectedResult);
    }
}
