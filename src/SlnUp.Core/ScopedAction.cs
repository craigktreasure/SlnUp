namespace SlnUp.Core;

public class ScopedAction : IDisposable
{
    private readonly Action action;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScopedAction"/> class.
    /// </summary>
    /// <param name="action">The action.</param>
    public ScopedAction(Action action)
        => this.action = action;

    public void Dispose()
    {
        this.action.Invoke();
        GC.SuppressFinalize(this);
    }
}
