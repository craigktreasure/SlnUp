namespace SlnUp.TestLibrary;

using System.IO.Abstractions;

/// <summary>
/// Class TemporaryFile.
/// </summary>
public static class TemporaryFile
{
    /// <summary>
    /// Creates a temporary file.
    /// </summary>
    /// <param name="fileName">The name of the file.</param>
    /// <returns><see cref="ScopedFile"/>.</returns>
    public static ScopedFile Create(string fileName)
        => Create(new FileSystem(), fileName);

    /// <summary>
    /// Creates a temporary file.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <returns><see cref="ScopedFile"/>.</returns>
    public static ScopedFile Create(IFileSystem fileSystem, string fileName)
    {
        ArgumentNullException.ThrowIfNull(fileSystem);

        string filePath = GetPathWithFileName(fileSystem, fileName);
        return new(fileSystem, filePath);
    }

    /// <summary>
    /// Creates a random temporary file.
    /// </summary>
    /// <returns><see cref="ScopedFile"/>.</returns>
    public static ScopedFile CreateRandom()
        => CreateRandom(new FileSystem());

    /// <summary>
    /// Creates a random temporary file.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <returns><see cref="ScopedFile"/>.</returns>
    public static ScopedFile CreateRandom(IFileSystem fileSystem)
        => Create(fileSystem, GetRandomFileName());

    /// <summary>
    /// Creates a random temporary file with the specified extension.
    /// </summary>
    /// <param name="extension">The file extension. No '.'.</param>
    /// <returns><see cref="ScopedFile" />.</returns>
    public static ScopedFile CreateRandomWithExtension(string extension)
        => Create(new FileSystem(), GetRandomFileNameWithExtension(extension));

    /// <summary>
    /// Creates a random temporary file.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="extension">The file extension. No '.'.</param>
    /// <returns><see cref="ScopedFile" />.</returns>
    public static ScopedFile CreateRandomWithExtension(IFileSystem fileSystem, string extension)
        => Create(fileSystem, GetRandomFileNameWithExtension(extension));

    /// <summary>
    /// Gets the path to a temporary file with the specified file name.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <returns>A path to a temporary file.</returns>
    public static string GetPathWithFileName(IFileSystem fileSystem, string fileName)
    {
        ArgumentNullException.ThrowIfNull(fileSystem);

        return fileSystem.Path.Combine(TemporaryDirectory.GetPath(fileSystem), fileName);
    }

    /// <summary>
    /// Gets a random file name.
    /// </summary>
    /// <returns>A random file name.</returns>
    public static string GetRandomFileName() => Guid.NewGuid().ToString();

    /// <summary>
    /// Gets a random file name with the specified extension.
    /// </summary>
    /// <param name="extension">The file extension. No '.'.</param>
    /// <returns>A random file name with the specified extension.</returns>
    public static string GetRandomFileNameWithExtension(string extension)
        => $"{GetRandomFileName()}.{extension}";

    /// <summary>
    /// Gets a path to a random temporary file. The file is not created.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <returns>A path to a random temporary file.</returns>
    public static string GetRandomFilePath(IFileSystem fileSystem)
        => GetPathWithFileName(fileSystem, GetRandomFileName());

    /// <summary>
    /// Gets a path to a random temporary file. The file is not created.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="extension">The file extension. No '.'.</param>
    /// <returns>A path to a random temporary file.</returns>
    public static string GetRandomFilePathWithExtension(IFileSystem fileSystem, string extension)
        => GetPathWithFileName(fileSystem, GetRandomFileNameWithExtension(extension));
}
