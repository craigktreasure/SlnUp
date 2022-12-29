namespace SlnUp.TestLibrary;

using System.IO.Abstractions;

using SlnUp.Core;

internal static class WorkingDirectory
{
    /// <summary>
    /// Gets the current working directory.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <returns>The path to the current working directory.</returns>
    public static string Get(IFileSystem fileSystem) => fileSystem.Directory.GetCurrentDirectory();

    /// <summary>
    /// Sets the working directory to the specified path.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="path">The path.</param>
    public static void Set(IFileSystem fileSystem, string path)
        => fileSystem.Directory.SetCurrentDirectory(path);

    /// <summary>
    /// Sets the working directory using a <see cref="ScopedAction"/> to enable reset upon disposal.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="path">The path.</param>
    /// <returns><see cref="IDisposable"/>.</returns>
    public static IDisposable SetScoped(IFileSystem fileSystem, string path)
    {
        string currentPath = Get(fileSystem);
        Set(fileSystem, path);

        return new ScopedAction(() => Set(fileSystem, currentPath));
    }
}
