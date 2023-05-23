namespace VisualStudio.VersionScraper;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

using HtmlAgilityPack;

using SlnUp.Core;
using SlnUp.Core.Extensions;

using VisualStudio.VersionScraper.Extensions;

internal sealed class VisualStudioVersionDocScraper
{
    private const string buildNumberColumnName = "Build Number";

    private const string buildVersionColumnName = "Build version";

    private const string channelColumnName = "Channel";

    private const string releaseDateColumnName = "Release Date";

    private const string versionColumnName = "Version";

    private const string vs2017VersionsDocUrl = "https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2017/install/visual-studio-build-numbers-and-release-dates";

    private const string vs2019VersionsDocUrl = "https://learn.microsoft.com/en-us/visualstudio/releases/2019/history";

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
        HashSet<VisualStudioVersion> versions = new();
        versions.AddRange(this.ScrapeVisualStudioVersions(vsCurrentVersionsDocUrl, "VSCurrentVersionCache"));
        versions.AddRange(this.ScrapeVisualStudioVersions(vs2019VersionsDocUrl, "VS2019VersionCache"));
        versions.AddRange(this.ScrapeVisualStudioVersions(vs2017VersionsDocUrl, "VS2017VersionCache"));
        return versions;
    }

    private static VisualStudioVersion GetVersionDetailFromRow(RowData row)
    {
        string versionInput = row.Version;
        string channel = row.Channel ?? string.Empty;

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

        if (!Version.TryParse(row.BuildNumber, out Version? buildVersion))
        {
            throw new InvalidDataException($"Third column did not contain a valid build version value: '{row.BuildNumber}'.");
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

    private static bool TryGetColumnIndices(HtmlNode table, out int versionIndex, out int releaseDateIndex, out int buildNumberIndex, out int? channelIndex)
    {
        const int notSet = -1;
        versionIndex = notSet;
        releaseDateIndex = notSet;
        buildNumberIndex = notSet;
        channelIndex = null;

        HtmlNodeCollection? headings = table.SelectNodes("thead//th");

        string[] columnNames = headings.Select(x => x.InnerText.Trim()).ToArray();

        for (int i = 0; i < columnNames.Length; i++)
        {
            string columnName = columnNames[i];

            if (versionIndex is notSet
                && columnName.Equals(versionColumnName, StringComparison.OrdinalIgnoreCase))
            {
                versionIndex = i;
            }
            else if (releaseDateIndex is notSet
                && columnName.Equals(releaseDateColumnName, StringComparison.OrdinalIgnoreCase))
            {
                releaseDateIndex = i;
            }
            else if (buildNumberIndex is notSet
                && (columnName.Equals(buildNumberColumnName, StringComparison.OrdinalIgnoreCase)
                    || columnName.Equals(buildVersionColumnName, StringComparison.OrdinalIgnoreCase)))
            {
                buildNumberIndex = i;
            }
            else if (!channelIndex.HasValue
                && columnName.Equals(channelColumnName, StringComparison.OrdinalIgnoreCase))
            {
                channelIndex = i;
            }
        }

        return versionIndex is not notSet
            && releaseDateIndex is not notSet
            && buildNumberIndex is not notSet;
    }

    private static bool TryGetTableData(
        HtmlNode table,
        [NotNullWhen(true)]
        out IReadOnlyCollection<RowData>? result)
    {
        result = null;

        if (table is null)
        {
            return false;
        }

        if (!TryGetColumnIndices(table, out int versionIndex, out int releaseDateIndex, out int buildNumberIndex, out int? channelIndex))
        {
            return false;
        }

        HtmlNodeCollection rows = table.SelectNodes("tbody/tr");

        if (rows is null || rows.Count is 0)
        {
            return false;
        }

        List<RowData> data = new();

        foreach (HtmlNode row in rows)
        {
            HtmlNodeCollection tds = row.SelectNodes("td");

            string version = tds[versionIndex].InnerText.Trim();
            string releaseDate = tds[releaseDateIndex].InnerText.Trim();
            string buildNumber = tds[buildNumberIndex].InnerText.Trim();
            string? channel = channelIndex.HasValue
                ? tds[channelIndex.Value].InnerText.Trim()
                : null;

            data.Add(new RowData(version, releaseDate, buildNumber, channel));
        }

        result = data;
        return true;
    }

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

        HtmlNodeCollection tables = doc.DocumentNode.SelectNodes("//table");

        if (tables.Count == 0)
        {
            throw new InvalidDataException("No tables were found.");
        }

        IReadOnlyCollection<RowData>? tableData = null;

        if (tables.Count > 1)
        {
            // Too many tables were found. We need to see if we can determine which one.
            foreach (HtmlNode table in tables)
            {
                if (TryGetTableData(table, out tableData))
                {
                    break;
                }
            }
        }
        else
        {
            TryGetTableData(tables.Single(), out tableData);
        }

        if (tableData is null)
        {
            throw new InvalidDataException("Unable to locate a suitable table or unable to retrieve table data.");
        }

        foreach (RowData rowData in tableData)
        {
            yield return GetVersionDetailFromRow(rowData);
        }
    }

    private sealed record RowData(string Version, string ReleaseDate, string BuildNumber, string? Channel);
}
