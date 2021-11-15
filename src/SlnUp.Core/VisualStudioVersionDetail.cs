namespace SlnUp.Core;

using Humanizer;
using System.IO.Abstractions;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Represents Visual Studio version details.
/// </summary>
/// <param name="VisualStudioVersion">The Visual Studio version moniker.</param>
/// <param name="Version">The Visual Studio version.</param>
/// <param name="BuildVersion">The Visual Studio build version.</param>
/// <param name="Channel">The Visual Studio version channel.</param>
/// <param name="IsPreview">Whether the Visual Studio version is a preview.</param>
public record VisualStudioVersionDetail(
    VisualStudioVersion VisualStudioVersion,
    Version Version,
    Version BuildVersion,
    string Channel = "",
    bool IsPreview = false)
{
    /// <summary>
    /// Gets the Visual Studio version full title.
    /// </summary>
    [JsonIgnore]
    public string VisualStudioFullTitle => this.ToString();

    /// <summary>
    /// Gets the Visual Studio version title.
    /// </summary>
    [JsonIgnore]
    public string VisualStudioTitle => this.VisualStudioVersion.Humanize().Transform(To.TitleCase);

    private static readonly JsonSerializerOptions serializerOptions = new()
    {
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
    /// <returns><see cref="IReadOnlyList{VisualStudioVersionDetail}"/>.</returns>
    /// <exception cref="InvalidDataException">The file did not contain valid json.</exception>
    public static IReadOnlyList<VisualStudioVersionDetail> FromJson(string json)
        => JsonSerializer.Deserialize<IReadOnlyList<VisualStudioVersionDetail>>(json, serializerOptions)
            ?? throw new InvalidDataException("The file did not contain valid json for version details.");

    /// <summary>
    /// Loads from json.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="filePath">The file path.</param>
    /// <returns><see cref="IReadOnlyList{VisualStudioVersionDetail}" />.</returns>
    public static IReadOnlyList<VisualStudioVersionDetail> FromJsonFile(IFileSystem fileSystem, string filePath)
        => FromJson(fileSystem.File.ReadAllText(filePath));

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode() => this.BuildVersion.GetHashCode();

    /// <summary>
    /// Serializes to json content.
    /// </summary>
    /// <param name="versions">The versions.</param>
    public static string ToJson(IEnumerable<VisualStudioVersionDetail> versions)
        => JsonSerializer.Serialize(versions, serializerOptions);

    /// <summary>
    /// Saves to json.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="versions">The versions.</param>
    /// <param name="filePath">The file path.</param>
    public static void ToJsonFile(IFileSystem fileSystem, IEnumerable<VisualStudioVersionDetail> versions, string filePath)
        => fileSystem.File.WriteAllText(filePath, ToJson(versions));

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    public override string ToString()
    {
        StringBuilder sb = new(this.VisualStudioTitle);

        if (this.IsPreview)
        {
            sb.Append(' ').Append(this.Version);

            if (!string.IsNullOrWhiteSpace(this.Channel))
            {
                sb.Append(' ').Append(this.Channel);
            }
        }
        else
        {
            sb.Append(' ')
              .Append(this.Version.Major)
              .Append('.')
              .Append(this.Version.Minor);
        }

        return sb.ToString();
    }
}
