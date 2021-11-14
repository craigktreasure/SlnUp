namespace SlnUp.Core;

using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

public static class RegexExtensions
{
    public static bool TryMatch(this Regex regex, string input, [NotNullWhen(true)] out Match? match)
    {
        match = null;
        Match myMatch = regex.Match(input);

        if (myMatch.Success)
        {
            match = myMatch;
        }

        return match is not null;
    }
}
