namespace SlnUp.Core;

using System.Text;

using Humanizer;

/// <summary>
/// Represents Visual Studio version details.
/// </summary>
public class VisualStudioVersion : IEquatable<VisualStudioVersion>
{
    /// <summary>
    /// Gets the Visual Studio build version.
    /// </summary>
    public Version BuildVersion { get; init; }

    /// <summary>
    /// Gets the Visual Studio channel.
    /// </summary>
    public string Channel { get; init; }

    /// <summary>
    /// Gets the Visual Studio full product title.
    /// </summary>
    public string FullProductTitle => this.ToString();

    /// <summary>
    /// Gets a value indicating whether the Visual Studio version is a preview.
    /// </summary>
    public bool IsPreview { get; init; }

    /// <summary>
    /// Gets the Visual Studio product.
    /// </summary>
    public VisualStudioProduct Product { get; init; }

    /// <summary>
    /// Gets the Visual Studio product title.
    /// </summary>
    public string ProductTitle => this.Product.Humanize().Transform(To.TitleCase);

    /// <summary>
    /// Gets the Visual Studio version.
    /// </summary>
    public Version Version { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="VisualStudioVersion"/> class.
    /// </summary>
    /// <param name="product">The Visual Studio product.</param>
    /// <param name="version">The Visual Studio version.</param>
    /// <param name="buildVersion">The Visual Studio build version.</param>
    /// <param name="channel">The Visual Studio channel.</param>
    /// <param name="isPreview">if set to <c>true</c>, the Visual Studio version is a preview.</param>
    public VisualStudioVersion(
        VisualStudioProduct product,
        Version version,
        Version buildVersion,
        string channel = "",
        bool isPreview = false)
    {
        this.Product = product;
        this.Version = version;
        this.BuildVersion = buildVersion;
        this.Channel = channel;
        this.IsPreview = isPreview;
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns><see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
    public bool Equals(VisualStudioVersion? other)
        => other is not null && this.BuildVersion == other.BuildVersion;

    /// <summary>
    /// Determines whether the specified <see cref="object" /> is equal to this instance.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
        => obj is VisualStudioVersion visualStudioVersion && this.Equals(visualStudioVersion);

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
