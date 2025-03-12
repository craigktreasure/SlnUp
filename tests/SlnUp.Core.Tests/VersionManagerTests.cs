namespace SlnUp.Core.Tests;

using System.Diagnostics.CodeAnalysis;

using SlnUp.Json;

public class VersionManagerTests
{
    [Test]
    [SuppressMessage("Globalization", "CA1305:Specify IFormatProvider")]
    public async Task Construct()
    {
        // Act
        VersionManager versionManager = new();

        // Assert
        foreach (VisualStudioProduct product in Enum.GetValues<VisualStudioProduct>())
        {
            if (product is VisualStudioProduct.Unknown)
            {
                continue;
            }

            VisualStudioVersion? version = versionManager.FromVersionParameter(((int)product).ToString());
            await Assert.That(version).IsNotNull();
        }
    }

    [Test]
    [SuppressMessage("Globalization", "CA1305:Specify IFormatProvider")]
    public async Task Construct_Versions()
    {
        // Arrange
        IReadOnlyList<VisualStudioVersion> versions =
        [
            new VisualStudioVersion(VisualStudioProduct.VisualStudio2022, Version.Parse("17.2.5"), Version.Parse("17.2.32616.157"), "Release", false),
        ];

        // Act
        VersionManager versionManager = new(versions);

        // Assert
        await Assert.That(versionManager.FromVersionParameter(((int)VisualStudioProduct.VisualStudio2022).ToString())).IsNotNull();
        await Assert.That(versionManager.FromVersionParameter(((int)VisualStudioProduct.VisualStudio2017).ToString())).IsNull();
    }

    [Test]
    [Arguments(null, false, null)]
    [Arguments("", false, null)]
    [Arguments(" ", false, null)]
    [Arguments("0", false, null)]
    [Arguments("0.0", false, null)]
    [Arguments("0.0.0", false, null)]
    [Arguments("0.0.0.0", false, null)]
    [Arguments("15.2", true, "15.0.26430.16")]
    [Arguments("15.2.5", true, "15.0.26430.15")]
    [Arguments("15.99", false, null)]
    [Arguments("15.2.99", false, null)]
    [Arguments("2017", true, "15.9.28307.1778")]
    [Arguments("16.9", true, "16.9.32106.192")]
    [Arguments("16.7.21", true, "16.7.31828.227")]
    [Arguments("16.99", false, null)]
    [Arguments("16.7.99", false, null)]
    [Arguments("2019", true, "16.11.32106.194")]
    [Arguments("17.0", true, "17.0.32112.339")]
    [Arguments("17.0.0", true, "17.0.31903.59")]
    [Arguments("17.99", false, null)]
    [Arguments("17.0.99", false, null)]
    [Arguments("18.0", true, "18.0.11205.157")]
    [Arguments("18.0.0", true, "18.0.11205.157")]
    [Arguments("18.0.1", false, null)]
    [Arguments("2022", true, "17.0.32112.339")]
    [Arguments("2026", true, "18.0.11205.157")]
    public async Task FromVersionParameter(string? input, bool expectFound, string? expectedBuildVersion)
    {
        // Arrange
        if (expectFound && expectedBuildVersion is null)
        {
            throw new ArgumentNullException(nameof(expectedBuildVersion), "We expect a value to be found but an expected build version wasn't provided.");
        }

        VersionManager versionManager = VersionManagerJsonHelper.LoadFromEmbeddedResource(typeof(VersionManagerTests).Assembly, "TestVersions.json");

        // Act
        VisualStudioVersion? version = versionManager.FromVersionParameter(input);

        // Assert
        if (!expectFound)
        {
            await Assert.That(version).IsNull();
        }
        else
        {
            await Assert.That(version).IsNotNull();
            await Assert.That(version!.BuildVersion).IsEqualTo(Version.Parse(expectedBuildVersion!));
        }
    }

    [Test]
    [Arguments(null, false)]
    [Arguments("", false)]
    [Arguments(" ", false)]
    [Arguments("0", false)]
    [Arguments("0.0", true)]
    [Arguments("0.0.0", true)]
    [Arguments("0.0.0.0", false)]
    [Arguments("2022", false)]
    [Arguments("2026", false)]
    public async Task TryParseVisualStudioVersion(string? input, bool expectedResult)
    {
        // Act
        bool result = VersionManager.TryParseVisualStudioVersion(input, out Version? version);

        // Assert
        await Assert.That(result).IsEqualTo(expectedResult);
    }
}
