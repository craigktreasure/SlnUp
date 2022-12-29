namespace SlnUp.TestLibrary;

using System.IO.Abstractions;

using SlnUp.Core;

using Treasure.Utils;

/// <summary>
/// Class ScopedDirectory.
/// Implements the <see cref="IDisposable" />
/// </summary>
/// <seealso cref="IDisposable" />
public class ScopedDirectory : IDisposable
{
    private readonly ScopedAction cleanupAction;

    private bool disposedValue;

    /// <summary>
    /// Gets the file system.
    /// </summary>
    public IFileSystem FileSystem { get; }

    /// <summary>
    /// Gets the directory path.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScopedFile"/> class.
    /// </summary>
    /// <param name="path">The directory path.</param>
    public ScopedDirectory(string path)
        : this(new FileSystem(), path)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScopedFile"/> class.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="path">The directory path.</param>
    public ScopedDirectory(IFileSystem fileSystem, string path)
    {
        this.FileSystem = Argument.NotNull(fileSystem);
        this.Path = Argument.NotNullOrWhiteSpace(path);

        if (!fileSystem.Directory.Exists(path))
        {
            fileSystem.Directory.CreateDirectory(path);
        }

        this.cleanupAction = new ScopedAction(this.Cleanup);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Gets the path to a temporary file with the specified file name.
    /// </summary>
    /// <param name="fileName">The name of the file.</param>
    /// <returns>A path to a temporary file.</returns>
    public string GetPathWithFileName(string fileName) => this.FileSystem.Path.Combine(this.Path, fileName);

    /// <summary>
    /// Gets a path to a random temporary file. The file is not created.
    /// </summary>
    /// <returns>A path to a random temporary file.</returns>
    public string GetRandomFilePath()
        => this.GetPathWithFileName(TemporaryFile.GetRandomFileName());

    /// <summary>
    /// Gets a path to a random temporary file. The file is not created.
    /// </summary>
    /// <param name="extension">The file extension. No '.'.</param>
    /// <returns>A path to a random temporary file.</returns>
    public string GetRandomFilePathWithExtension(string extension)
        => this.GetPathWithFileName(TemporaryFile.GetRandomFileNameWithExtension(extension));

    /// <summary>
    /// Sets the directory as the working directory using scoped behavior.
    /// </summary>
    /// <returns><see cref="IDisposable"/>.</returns>
    public IDisposable SetAsScopedWorkingDirectory()
        => WorkingDirectory.SetScoped(this.FileSystem, this.Path);

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing">
    /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                this.cleanupAction.Dispose();
            }

            this.disposedValue = true;
        }
    }

    private void Cleanup()
    {
        if (this.FileSystem.Directory.Exists(this.Path))
        {
            this.FileSystem.Directory.Delete(this.Path, recursive: true);
        }
    }
}
