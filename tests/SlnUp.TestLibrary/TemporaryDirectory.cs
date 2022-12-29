namespace SlnUp.TestLibrary;

using System.IO.Abstractions;

using Treasure.Utils;

/// <summary>
/// Class TemporaryDirectory.
/// </summary>
public static class TemporaryDirectory
{
    /// <summary>
    /// Creates a temporary directory.
    /// </summary>
    /// <param name="directoryName">The name of the directory.</param>
    /// <returns><see cref="ScopedDirectory"/>.</returns>
    public static ScopedDirectory Create(string directoryName)
        => Create(new FileSystem(), directoryName);

    /// <summary>
    /// Creates a temporary directory.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="directoryName">The name of the directory.</param>
    /// <returns><see cref="ScopedDirectory"/>.</returns>
    public static ScopedDirectory Create(IFileSystem fileSystem, string directoryName)
    {
        Argument.NotNull(fileSystem);

        string directoryPath = GetPathWithName(fileSystem, directoryName);

        return new(fileSystem, directoryPath);
    }

    /// <summary>
    /// Creates a random temporary directory.
    /// </summary>
    /// <returns><see cref="ScopedDirectory"/>.</returns>
    public static ScopedDirectory CreateRandom()
        => CreateRandom(new FileSystem());

    /// <summary>
    /// Creates a random temporary directory.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <returns><see cref="ScopedDirectory"/>.</returns>
    public static ScopedDirectory CreateRandom(IFileSystem fileSystem)
        => Create(fileSystem, GetRandomDirectoryName());

    /// <summary>
    /// Gets the path to the temporary directory.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <returns><see cref="string"/>.</returns>
    public static string GetPath(IFileSystem fileSystem)
    {
        ArgumentNullException.ThrowIfNull(fileSystem);

        return fileSystem.Path.GetTempPath();
    }

    /// <summary>
    /// Gets the path to a temporary directory with the specified name.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="directoryName">Name of the directory.</param>
    /// <returns><see cref="string"/>.</returns>
    public static string GetPathWithName(IFileSystem fileSystem, string directoryName)
    {
        ArgumentNullException.ThrowIfNull(fileSystem);
        Argument.NotNullOrWhiteSpace(directoryName);

        return fileSystem.Path.Combine(GetPath(fileSystem), directoryName);
    }

    /// <summary>
    /// Gets a random directory name.
    /// </summary>
    /// <returns>A random directory name.</returns>
    public static string GetRandomDirectoryName() => Guid.NewGuid().ToString();
}
