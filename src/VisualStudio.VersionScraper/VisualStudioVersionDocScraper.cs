namespace VisualStudio.VersionScraper;

using System.Globalization;
using System.Text.RegularExpressions;

using HtmlAgilityPack;

using SlnUp.Core;
using SlnUp.Core.Extensions;

internal sealed class VisualStudioVersionDocScraper
{
    private const string vs2017VersionsDocUrl = "https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2017/install/visual-studio-build-numbers-and-release-dates";

    private const string vsCurrentVersionsDocUrl = "https://docs.microsoft.com/en-us/visualstudio/install/visual-studio-build-numbers-and-release-dates";

    private static readonly Regex vs15PreviewVersionMatcher = new(
        @"(?<version>\d+\.\d+\.?\d*) (?<channel>Preview \d\.?\d*)",
        RegexOptions.Compiled);

    private readonly bool useCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="VisualStudioVersionDocScraper"/> class.
    /// </summary>
    /// <param name="useCache">if set to <c>true</c>, use a cache.</param>
    public VisualStudioVersionDocScraper(bool useCache = true)
        => this.useCache = useCache;

    /// <summary>
    /// Scrapes the visual studio versions from the documentation.
    /// </summary>
    /// <returns><see cref="IEnumerable{VisualStudioVersion}"/>.</returns>
    public IEnumerable<VisualStudioVersion> ScrapeVisualStudioVersions()
    {
        IEnumerable<VisualStudioVersion> currentVersions = this.ScrapeVisualStudioVersions(vsCurrentVersionsDocUrl, "VSCurrentVersionCache");
        IEnumerable<VisualStudioVersion> previousVersions = this.ScrapeVisualStudioVersions(vs2017VersionsDocUrl, "VS2017VersionCache");
        return currentVersions.Concat(previousVersions);
    }

    private static VisualStudioVersion GetVersionDetailFromRow(HtmlNode row)
    {
        HtmlNodeCollection tds = row.SelectNodes("td");

        if (tds.Count != 4)
        {
            throw new InvalidDataException($"Unexpected number of columns in table: {tds.Count}.");
        }

        string versionInput = tds[0].InnerText.Trim();
        string channel = tds[1].InnerText.Trim();

        if (vs15PreviewVersionMatcher.TryMatch(versionInput, out Match? match)
            && Version.TryParse(match.Groups["version"].Value, out Version? version))
        {
            if (version.Build == -1)
            {
                // Ensure a 3-part build number.
                version = new Version(version.Major, version.Minor, 0);
            }

            // The VS 2017 version and channel can be specified differently for preview versions.
            channel = match.Groups["channel"].Value;
        }
        else if (!Version.TryParse(versionInput, out version))
        {
            throw new InvalidDataException($"First column did not contain a valid version value: '{versionInput}'.");
        }

        if (!Version.TryParse(tds[3].InnerText.Trim(), out Version? buildVersion))
        {
            throw new InvalidDataException($"Third column did not contain a valid build version value: '{tds[3].InnerText}'.");
        }

        VisualStudioProduct vsVersion = GetVisualStudioVersion(version);
        bool isPreview = channel.StartsWith("Preview", StringComparison.OrdinalIgnoreCase);

        return new VisualStudioVersion(vsVersion, version, buildVersion, channel, isPreview);
    }

    private static VisualStudioProduct GetVisualStudioVersion(Version version)
        => GetVisualStudioVersion(version.Major);

    private static VisualStudioProduct GetVisualStudioVersion(int majorVersion) => majorVersion switch
    {
        15 => VisualStudioProduct.VisualStudio2017,
        16 => VisualStudioProduct.VisualStudio2019,
        17 => VisualStudioProduct.VisualStudio2022,
        _ => VisualStudioProduct.Unknown
    };

    private HtmlDocument LoadVisualStudioVersionDocument(string url, string cacheFolderName)
    {
        string cachePath = Path.Join(Path.GetTempPath(), cacheFolderName);
        HtmlWeb web = new()
        {
            CachePath = cachePath,
            UsingCache = this.useCache,
        };

        UriBuilder uriBuilder = new(url);
        if (this.useCache)
        {
            // Add a cache query parameter.
            uriBuilder.Query = $"?cache={DateTime.Now.Date.ToString("MM-dd-yyyy", CultureInfo.CurrentCulture)}";
        }

        HtmlDocument doc = web.Load(uriBuilder.Uri);

        return doc;
    }

    private IEnumerable<VisualStudioVersion> ScrapeVisualStudioVersions(string url, string cacheFolderName)
    {
        HtmlDocument doc = this.LoadVisualStudioVersionDocument(url, cacheFolderName);

        HtmlNodeCollection rows = doc.DocumentNode.SelectNodes("//table/tbody/tr");

        if (rows is null || rows.Count is 0)
        {
            throw new InvalidDataException("No rows were found.");
        }

        foreach (HtmlNode row in rows)
        {
            yield return GetVersionDetailFromRow(row);
        }
    }
}
