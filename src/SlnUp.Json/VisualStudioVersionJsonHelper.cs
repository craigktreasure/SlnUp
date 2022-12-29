namespace SlnUp.Json;

using System.IO.Abstractions;
using System.Text.Json;
using System.Text.Json.Serialization;

using SlnUp.Core;

using Treasure.Utils;

/// <summary>
/// A helper when converting <see cref="VisualStudioVersion" /> data to and from json.
/// </summary>
public static class VisualStudioVersionJsonHelper
{
    private static readonly JsonSerializerOptions serializerOptions = new()
    {
        IgnoreReadOnlyProperties = true,
        WriteIndented = true,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };

    /// <summary>
    /// Deserializes version details from json content.
    /// </summary>
    /// <param name="json">The json.</param>
    /// <returns><see cref="IReadOnlyList{VisualStudioVersion}"/>.</returns>
    /// <exception cref="InvalidDataException">The file did not contain valid json.</exception>
    public static IReadOnlyList<VisualStudioVersion> FromJson(string json)
        => JsonSerializer.Deserialize<IReadOnlyList<VisualStudioVersion>>(json, serializerOptions)
            ?? throw new InvalidDataException("The file did not contain valid json for version details.");

    /// <summary>
    /// Loads from json.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="filePath">The file path.</param>
    /// <returns><see cref="IReadOnlyList{VisualStudioVersion}" />.</returns>
    public static IReadOnlyList<VisualStudioVersion> FromJsonFile(IFileSystem fileSystem, string filePath)
        => FromJson(Argument.NotNull(fileSystem).File.ReadAllText(filePath));

    /// <summary>
    /// Serializes to json content.
    /// </summary>
    /// <param name="versions">The versions.</param>
    public static string ToJson(IEnumerable<VisualStudioVersion> versions)
        => JsonSerializer.Serialize(versions, serializerOptions);

    /// <summary>
    /// Saves to json.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="versions">The versions.</param>
    /// <param name="filePath">The file path.</param>
    public static void ToJsonFile(IFileSystem fileSystem, IEnumerable<VisualStudioVersion> versions, string filePath)
        => Argument.NotNull(fileSystem).File.WriteAllText(filePath, ToJson(versions));
}
