namespace SlnUp.Core;

/// <summary>
/// Class ScopedAction.
/// Implements the <see cref="IDisposable" />
/// </summary>
/// <seealso cref="IDisposable" />
public class ScopedAction : IDisposable
{
    private readonly Action action;

    private bool disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScopedAction"/> class.
    /// </summary>
    /// <param name="action">The action.</param>
    public ScopedAction(Action action)
        => this.action = action;

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
                this.action?.Invoke();
            }

            this.disposedValue = true;
        }
    }
}
