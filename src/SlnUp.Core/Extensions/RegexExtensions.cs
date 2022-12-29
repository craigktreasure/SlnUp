namespace SlnUp.Core.Extensions;

using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

/// <summary>
/// Class RegexExtensions.
/// </summary>
public static class RegexExtensions
{
    /// <summary>
    /// Tries to match the specified <see cref="Regex"/>.
    /// </summary>
    /// <param name="regex">The regex.</param>
    /// <param name="input">The input.</param>
    /// <param name="match">The match.</param>
    /// <returns><c>true</c> if the match was successful, <c>false</c> otherwise.</returns>
    public static bool TryMatch(this Regex regex, string input, [NotNullWhen(true)] out Match? match)
    {
        ArgumentNullException.ThrowIfNull(regex);

        match = null;
        Match myMatch = regex.Match(input);

        if (myMatch.Success)
        {
            match = myMatch;
        }

        return match is not null;
    }
}
