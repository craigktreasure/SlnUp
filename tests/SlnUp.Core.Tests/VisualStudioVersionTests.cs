namespace SlnUp.Core.Tests;

public class VisualStudioVersionTests
{
    private static readonly IReadOnlyList<VisualStudioVersion> commonVersions = new[]
    {
        new VisualStudioVersion(
            VisualStudioProduct.VisualStudio2019,
            Version.Parse("16.11.6"),
            Version.Parse("16.11.31829.152"),
            "Release"),
        new VisualStudioVersion(
            VisualStudioProduct.VisualStudio2022,
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
        VisualStudioVersion version = new(
            VisualStudioProduct.VisualStudio2022,
            Version.Parse(expectedVersion),
            Version.Parse(expectedBuildVersion),
            expectedChannel);

        // Assert
        version.Channel.Should().Be(expectedChannel);
        version.Version.ToString().Should().Be(expectedVersion);
        version.BuildVersion.ToString().Should().Be(expectedBuildVersion);
        version.ProductTitle.Should().Be(expectedVersionTitle);
        version.FullProductTitle.Should().Be(expectedFullVersionTitle);
    }

    [Fact]
    public void Constructor_WithPreview()
    {
        // Arrange
        const string expectedChannel = "Preview 1";
        const string expectedFullVersionTitle = "Visual Studio 2022 17.0.1 Preview 1";
        const string expectedVersionTitle = "Visual Studio 2022";
        const string expectedVersion = "17.0.1";
        const string expectedBuildVersion = "17.1.31903.286";

        // Act
        VisualStudioVersion version = new(
            VisualStudioProduct.VisualStudio2022,
            Version.Parse(expectedVersion),
            Version.Parse(expectedBuildVersion),
            expectedChannel,
            IsPreview: true);

        // Assert
        version.Channel.Should().Be(expectedChannel);
        version.Version.ToString().Should().Be(expectedVersion);
        version.BuildVersion.ToString().Should().Be(expectedBuildVersion);
        version.ProductTitle.Should().Be(expectedVersionTitle);
        version.FullProductTitle.Should().Be(expectedFullVersionTitle);
    }

    [Fact]
    public void GetHashCodeMethod()
    {
        // Arrange
        VisualStudioVersion version = commonVersions[0];
        int expectedHashCode = version.BuildVersion.GetHashCode();

        // Act
        int hashCode = version.GetHashCode();

        // Assert
        hashCode.Should().Be(expectedHashCode);
    }
}
