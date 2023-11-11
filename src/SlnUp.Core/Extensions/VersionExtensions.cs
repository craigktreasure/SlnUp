namespace SlnUp.Core.Extensions;

using System;

using Treasure.Utils;

/// <summary>
/// Class VersionExtensions.
/// </summary>
public static class VersionExtensions
{
    /// <summary>
    /// Determines whether the versions have the same major and minor version numbers.
    /// </summary>
    /// <param name="version">The version.</param>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if the major and minor versions are the same; otherwise, <c>false</c>.</returns>
    public static bool HasSameMajorMinor(this Version version, Version other)
        => Argument.NotNull(version).Major == Argument.NotNull(other).Major && version.Minor == other.Minor;

    /// <summary>
    /// Determines if the version declares a 2-part version number.
    /// </summary>
    /// <param name="version">The version.</param>
    /// <returns><c>true</c> if the version declares a 2-part version number, <c>false</c> otherwise.</returns>
    public static bool Is2PartVersion(this Version version)
    {
        Argument.NotNull(version);
        return version is { Major: >= 0, Minor: >= 0, Build: -1, Revision: -1 };
    }

    /// <summary>
    /// Determines if the version declares a 3-part version number.
    /// </summary>
    /// <param name="version">The version.</param>
    /// <returns><c>true</c> if the version declares a 3-part version number, <c>false</c> otherwise.</returns>
    public static bool Is3PartVersion(this Version version)
    {
        Argument.NotNull(version);
        return version is { Major: >= 0, Minor: >= 0, Build: >= 0, Revision: -1 };
    }

    /// <summary>
    /// Determines if the version declares a 4-part version number.
    /// </summary>
    /// <param name="version">The version.</param>
    /// <returns><c>true</c> if the version declares a 4-part version number, <c>false</c> otherwise.</returns>
    public static bool Is4PartVersion(this Version version)
    {
        Argument.NotNull(version);
        return version is { Major: >= 0, Minor: >= 0, Build: >= 0, Revision: >= 0 };
    }
}
