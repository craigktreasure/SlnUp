namespace SlnUp.Json;

using System.Reflection;

using SlnUp.Core;
using SlnUp.Json.Extensions;

/// <summary>
/// A helper when loading a <see cref="VersionManager" /> from json.
/// </summary>
public static class VersionManagerJsonHelper
{
    /// <summary>
    /// Loads a <see cref="VersionManager" /> from an embedded file resource.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    /// <param name="filePath">The file path.</param>
    /// <returns><see cref="VersionManager" />.</returns>
    public static VersionManager LoadFromEmbeddedResource(Assembly assembly, string filePath)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        string jsonContent = assembly.GetEmbeddedFileResourceContent(filePath);

        IReadOnlyList<VisualStudioVersion> versions = VisualStudioVersionJsonHelper.FromJson(jsonContent);

        return new VersionManager(versions);
    }
}
