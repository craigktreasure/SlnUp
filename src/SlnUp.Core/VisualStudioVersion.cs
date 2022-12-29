namespace SlnUp.Core;

using System.Text;

using Humanizer;

/// <summary>
/// Represents Visual Studio version details.
/// </summary>
/// <param name="Product">The Visual Studio product.</param>
/// <param name="Version">The Visual Studio version.</param>
/// <param name="BuildVersion">The Visual Studio build version.</param>
/// <param name="Channel">The Visual Studio version channel.</param>
/// <param name="IsPreview">Whether the Visual Studio version is a preview.</param>
public record VisualStudioVersion(
    VisualStudioProduct Product,
    Version Version,
    Version BuildVersion,
    string Channel = "",
    bool IsPreview = false)
{
    /// <summary>
    /// Gets the Visual Studio full product title.
    /// </summary>
    public string FullProductTitle => this.ToString();

    /// <summary>
    /// Gets the Visual Studio product title.
    /// </summary>
    public string ProductTitle => this.Product.Humanize().Transform(To.TitleCase);

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
        StringBuilder sb = new(this.ProductTitle);

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
