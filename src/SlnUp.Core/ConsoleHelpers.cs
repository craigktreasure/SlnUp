namespace SlnUp.Core;

using System;

/// <summary>
/// Class ConsoleHelpers.
/// </summary>
public static class ConsoleHelpers
{
    /// <summary>
    /// Configures the console foreground color for errors for the lifetime of the <see cref="IDisposable"/>.
    /// </summary>
    /// <returns><see cref="IDisposable"/>.</returns>
    public static IDisposable WithError()
        => WithForegroundColor(ConsoleColor.Red);

    /// <summary>
    /// Configures the console foreground color for the lifetime of the <see cref="IDisposable"/>.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns><see cref="IDisposable"/>.</returns>
    public static IDisposable WithForegroundColor(ConsoleColor color)
    {
        ConsoleColor originalColor = Console.ForegroundColor;
        Console.ForegroundColor = color;

        return new ScopedAction(() => Console.ForegroundColor = originalColor);
    }
}
