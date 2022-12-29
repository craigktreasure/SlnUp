namespace SlnUp.TestLibrary.Extensions;

using System.IO.Abstractions.TestingHelpers;

/// <summary>
/// Class StringExtensions.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Converts to a cross-platform path from a Windows path.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns><see cref="string"/>.</returns>
    public static string ToCrossPlatformPath(this string path)
        => MockUnixSupport.Path(path);

    /// <summary>
    /// Converts to cross-platform paths from Windows paths.
    /// </summary>
    /// <param name="paths">The paths.</param>
    /// <returns><see cref="string"/>.</returns>
    public static IEnumerable<string> ToCrossPlatformPath(this IEnumerable<string> paths)
        => paths.Select(MockUnixSupport.Path);
}
