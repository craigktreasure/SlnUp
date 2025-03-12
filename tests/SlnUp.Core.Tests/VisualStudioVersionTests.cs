namespace SlnUp.Core.Tests;

public class VisualStudioVersionTests
{
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
            "Release")
    ];

    [Test]
    public async Task Constructor()
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
        await Assert.That(version.Channel).IsEqualTo(expectedChannel);
        await Assert.That(version.Version.ToString()).IsEqualTo(expectedVersion);
        await Assert.That(version.BuildVersion.ToString()).IsEqualTo(expectedBuildVersion);
        await Assert.That(version.ProductTitle).IsEqualTo(expectedVersionTitle);
        await Assert.That(version.FullProductTitle).IsEqualTo(expectedFullVersionTitle);
    }

    [Test]
    public async Task Constructor_WithPreview()
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
            isPreview: true);

        // Assert
        await Assert.That(version.Channel).IsEqualTo(expectedChannel);
        await Assert.That(version.Version.ToString()).IsEqualTo(expectedVersion);
        await Assert.That(version.BuildVersion.ToString()).IsEqualTo(expectedBuildVersion);
        await Assert.That(version.ProductTitle).IsEqualTo(expectedVersionTitle);
        await Assert.That(version.FullProductTitle).IsEqualTo(expectedFullVersionTitle);
    }

    [Test]
    public async Task Equals_NullParameter_ReturnsFalse()
    {
        // Arrange
        VisualStudioVersion version = commonVersions[0];
        VisualStudioVersion? other = null;

        // Act
        bool result = version.Equals(other);

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Equals_SameReference_ReturnsTrue()
    {
        // Arrange
        VisualStudioVersion version = commonVersions[0];
        VisualStudioVersion? other = commonVersions[0];

        // Act
        bool result = version.Equals(other);

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task Equals_SameValue_ReturnsTrue()
    {
        // Arrange
        VisualStudioVersion version = new(
            VisualStudioProduct.VisualStudio2022,
            Version.Parse("17.0.0"),
            Version.Parse("17.0.31903.59"),
            "Release");
        VisualStudioVersion? other = new(
            VisualStudioProduct.VisualStudio2022,
            Version.Parse("17.0.0"),
            Version.Parse("17.0.31903.59"),
            "Release");

        // Act
        bool result = version.Equals(other);

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task Equals_WithObject_NullParameter_ReturnsFalse()
    {
        // Arrange
        VisualStudioVersion version = commonVersions[0];
        object? other = null;

        // Act
        bool result = version.Equals(other);

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Equals_WithObject_SameValue_ReturnsTrue()
    {
        // Arrange
        VisualStudioVersion version = new(
            VisualStudioProduct.VisualStudio2022,
            Version.Parse("17.0.0"),
            Version.Parse("17.0.31903.59"),
            "Release");
        object? other = new VisualStudioVersion(
            VisualStudioProduct.VisualStudio2022,
            Version.Parse("17.0.0"),
            Version.Parse("17.0.31903.59"),
            "Release");

        // Act
        bool result = version.Equals(other);

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task GetHashCodeMethod()
    {
        // Arrange
        VisualStudioVersion version = commonVersions[0];
        int expectedHashCode = version.BuildVersion.GetHashCode();

        // Act
        int hashCode = version.GetHashCode();

        // Assert
        await Assert.That(hashCode).IsEqualTo(expectedHashCode);
    }
}
