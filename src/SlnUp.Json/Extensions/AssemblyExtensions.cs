namespace SlnUp.Json.Extensions;

using System;
using System.Reflection;

internal static class AssemblyExtensions
{
    /// <summary>
    /// Gets the content of the embedded file resource.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    /// <param name="filePath">The file path.</param>
    /// <returns><see cref="string"/>.</returns>
    /// <exception cref="ArgumentException">Resource not found in assembly - filePath</exception>
    public static string GetEmbeddedFileResourceContent(this Assembly assembly, string filePath)
    {
        using Stream stream = assembly.GetManifestResourceStream(filePath)
            ?? throw new ArgumentException("Resource not found in assembly", nameof(filePath));
        using StreamReader streamReader = new(stream);
        return streamReader.ReadToEnd();
    }
}
