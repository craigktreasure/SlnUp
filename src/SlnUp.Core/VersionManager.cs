namespace SlnUp.Core;

using System.Diagnostics.CodeAnalysis;

using SlnUp.Core.Extensions;

/// <summary>
/// Manages versions of Visual Studio.
/// </summary>
public partial class VersionManager
{
    private readonly IReadOnlyList<VisualStudioVersion> versions;

    /// <summary>
    /// Initializes a new instance of the <see cref="VersionManager"/> class.
    /// </summary>
    public VersionManager()
        : this(GetDefaultVersions())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VersionManager"/> class.
    /// </summary>
    /// <param name="versions">The versions.</param>
    public VersionManager(IReadOnlyList<VisualStudioVersion> versions)
        => this.versions = versions;

    /// <summary>
    /// Tries to parse a Visual Studio version (x.x[.x]) from the specified input.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="version">The version.</param>
    /// <returns><c>true</c> if a valid Visual Studio version was parsed, <c>false</c> otherwise.</returns>
    public static bool TryParseVisualStudioVersion(string? input, [NotNullWhen(true)] out Version? version)
    {
        version = null;

        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        if (Version.TryParse(input, out Version? potentialVersion)
            && (potentialVersion.Is2PartVersion() || potentialVersion.Is3PartVersion()))
        {
            version = potentialVersion;
        }

        return version is not null;
    }

    /// <summary>
    /// Gets a Visual Studio version from the specified input.
    /// Supported inputs are a 2 or 3-part version number or a valid product year.
    /// </summary>
    /// <param name="versionInput">The version input.</param>
    /// <returns><see cref="Nullable{VisualStudioVersion}"/>.</returns>
    public VisualStudioVersion? FromVersionParameter(string? versionInput)
    {
        if (versionInput is null)
        {
            return null;
        }

        if (TryParseVisualStudioVersion(versionInput, out Version? parsedVersion))
        {
            return this.FindLatestMatchingVersion(parsedVersion);
        }
        else if (TryParseVisualStudioProduct(versionInput, out VisualStudioProduct product))
        {
            return this.FindLatestMatchingVersion(product);
        }

        return null;
    }

    private static VisualStudioProduct GetVersionFromProductYear(int productYear)
            => productYear switch
            {
                2017 => VisualStudioProduct.VisualStudio2017,
                2019 => VisualStudioProduct.VisualStudio2019,
                2022 => VisualStudioProduct.VisualStudio2022,
                _ => VisualStudioProduct.Unknown,
            };

    private static bool TryParseVisualStudioProduct(string input, out VisualStudioProduct product)
    {
        product = VisualStudioProduct.Unknown;

        if (int.TryParse(input, out int productYear))
        {
            product = GetVersionFromProductYear(productYear);
        }

        return product is not VisualStudioProduct.Unknown;
    }

    private VisualStudioVersion? FindLatestMatchingVersion(Version version)
    {
        VisualStudioVersion? versionResolved = null;

        if (version.Is3PartVersion())
        {
            // Expect an exact match.
            versionResolved = this.versions.FirstOrDefault(v => v.Version == version);
        }

        if (version.Is2PartVersion())
        {
            // Expect a major.minor match.
            versionResolved = this.versions.Where(v => v.Version.HasSameMajorMinor(version)).MaxBy(v => v.BuildVersion);
        }

        return versionResolved;
    }

    private VisualStudioVersion? FindLatestMatchingVersion(VisualStudioProduct product)
        => this.versions.Where(v => v.Product == product).MaxBy(v => v.BuildVersion);
}
