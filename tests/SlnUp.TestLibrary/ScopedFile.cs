namespace SlnUp.TestLibrary;

using System.IO.Abstractions;

using SlnUp.Core;

using Treasure.Utils;

/// <summary>
/// Class ScopedFile.
/// Implements the <see cref="IDisposable" />
/// </summary>
/// <seealso cref="IDisposable" />
public class ScopedFile : IDisposable
{
    private readonly ScopedAction cleanupAction;

    private bool disposedValue;

    /// <summary>
    /// Gets the file system.
    /// </summary>
    public IFileSystem FileSystem { get; }

    /// <summary>
    /// Gets the file path.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScopedFile"/> class.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    public ScopedFile(string filePath)
        : this(new FileSystem(), filePath)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScopedFile"/> class.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="filePath">The file path.</param>
    public ScopedFile(IFileSystem fileSystem, string filePath)
    {
        this.FileSystem = Argument.NotNull(fileSystem);
        this.Path = Argument.NotNullOrWhiteSpace(filePath);

        if (!fileSystem.File.Exists(filePath))
        {
            fileSystem.File.WriteAllText(this.Path, string.Empty);
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
        if (this.FileSystem.File.Exists(this.Path))
        {
            this.FileSystem.File.Delete(this.Path);
        }
    }
}
