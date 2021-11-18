namespace SlnUp.Core;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

/// <summary>
/// Class Argument.
/// </summary>
public static class Argument
{
    /// <summary>
    /// Asserts the value is not null.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <param name="name">The name.</param>
    /// <returns>The object if not null.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static T NotNull<T>([NotNull] T? value, [CallerArgumentExpression("value")] string name = "") where T : class
    {
        if (value is null)
        {
            throw new ArgumentNullException(name);
        }

        return value;
    }

    /// <summary>
    /// Asserts the value is not null or empty.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="name">The name.</param>
    /// <returns><see cref="string"/>.</returns>
    /// <exception cref="ArgumentException">The value cannot be null or empty.</exception>
    public static string NotNullOrEmpty([NotNull] string? value, [CallerArgumentExpression("value")] string name = "")
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException("The value cannot be null or empty.", name);
        }

        return value;
    }

    /// <summary>
    /// Asserts the value is not null, empty, or white-space.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="name">The name.</param>
    /// <returns><see cref="string"/>.</returns>
    /// <exception cref="ArgumentException">The value cannot be null, empty, or white-space.</exception>
    public static string NotNullOrWhiteSpace([NotNull] string? value, [CallerArgumentExpression("value")] string name = "")
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("The value cannot be null, empty, or white-space.", name);
        }

        return value;
    }
}
