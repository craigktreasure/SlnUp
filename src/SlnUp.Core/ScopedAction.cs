namespace SlnUp.Core;

/// <summary>
/// Class ScopedAction.
/// Implements the <see cref="IDisposable" />
/// </summary>
/// <seealso cref="IDisposable" />
public sealed class ScopedAction : IDisposable
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
        if (!this.disposedValue)
        {
            this.action?.Invoke();
            this.disposedValue = true;
        }
    }
}
