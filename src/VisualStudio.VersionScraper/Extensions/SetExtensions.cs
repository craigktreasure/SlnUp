namespace VisualStudio.VersionScraper.Extensions;

internal static class SetExtensions
{
    public static void AddRange<T>(this ISet<T> collection, IEnumerable<T> values)
    {
        foreach (T value in values)
        {
            collection.Add(value);
        }
    }
}
