namespace SlnUp.Core;

using Humanizer;
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

    /// <summary>
    /// Loads from json.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <returns><see cref="IReadOnlyList{VisualStudioVersionDetail}"/>.</returns>
    /// <exception cref="System.IO.InvalidDataException">The file did not contain valid json.</exception>
    public static IReadOnlyList<VisualStudioVersionDetail> LoadFromJson(string filePath)
    {
        string json = File.ReadAllText(filePath);

        return JsonSerializer.Deserialize<IReadOnlyList<VisualStudioVersionDetail>>(json)
            ?? throw new InvalidDataException("The file did not contain valid json.");
    }

    /// <summary>
    /// Saves to json.
    /// </summary>
    /// <param name="versions">The versions.</param>
    /// <param name="filePath">The file path.</param>
    public static void SaveToJson(IEnumerable<VisualStudioVersionDetail> versions, string filePath)
    {
        string json = JsonSerializer.Serialize(versions);

        File.WriteAllText(filePath, json);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode() => this.BuildVersion.GetHashCode();

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    public override string ToString()
    {
        StringBuilder sb = new(this.VisualStudioTitle);
        sb.Append(' ')
          .Append(this.Version.Major)
          .Append('.')
          .Append(this.Version.Minor);

        if (!string.IsNullOrWhiteSpace(this.Channel))
        {
            sb.Append(' ').Append(this.Channel);
        }

        return sb.ToString();
    }
}
