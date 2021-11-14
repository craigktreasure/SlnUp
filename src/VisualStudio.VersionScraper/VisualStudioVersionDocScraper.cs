namespace VisualStudio.VersionScraper;

using HtmlAgilityPack;
using SlnUp.Core;
using System.Text.RegularExpressions;

internal class VisualStudioVersionDocScraper
{
    private const string vsVersionsDocUrl = "https://docs.microsoft.com/en-us/visualstudio/install/visual-studio-build-numbers-and-release-dates";

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
    /// <returns><see cref="IEnumerable{VisualStudioVersionDetail}"/>.</returns>
    public IEnumerable<VisualStudioVersionDetail> ScrapeVisualStudioVersions()
    {
        HtmlDocument doc = this.LoadVisualStudioVersionDocument();

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

    private static VisualStudioVersionDetail GetVersionDetailFromRow(HtmlNode row)
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

        VisualStudioVersion vsVersion = GetVisualStudioVersion(version);
        bool isPreview = channel.StartsWith("Preview", StringComparison.OrdinalIgnoreCase);

        return new VisualStudioVersionDetail(vsVersion, version, buildVersion, channel, isPreview);
    }

    private static VisualStudioVersion GetVisualStudioVersion(Version version)
        => GetVisualStudioVersion(version.Major);

    private static VisualStudioVersion GetVisualStudioVersion(int majorVersion) => majorVersion switch
    {
        15 => VisualStudioVersion.VisualStudio2017,
        16 => VisualStudioVersion.VisualStudio2019,
        17 => VisualStudioVersion.VisualStudio2022,
        _ => VisualStudioVersion.Unknown
    };

    private HtmlDocument LoadVisualStudioVersionDocument()
    {
        string cachePath = Path.Join(Path.GetTempPath(), "VSVersionCache");
        HtmlWeb web = new()
        {
            CachePath = cachePath,
            UsingCache = this.useCache,
            UsingCacheIfExists = this.useCache,
        };

        HtmlDocument doc = web.Load(vsVersionsDocUrl);

        return doc;
    }
}
