namespace SlnUp.CLI;

using System.CommandLine.Parsing;
using System.Globalization;

internal static class ArgumentParser
{
    private const string CannotParseArgumentOption = "Cannot parse argument '{0}' for option '{1}' as expected type '{2}'.";

    public static Version? ParseVersion(ArgumentResult result)
    {
        string tokenValue = result.Tokens.Single().Value;
        if (Version.TryParse(tokenValue, out Version? version))
        {
            return version;
        }

        result.ErrorMessage = string.Format(
            CultureInfo.InvariantCulture,
            CannotParseArgumentOption,
            tokenValue,
            result.Argument.Name,
            result.Argument.ValueType);
        return null;
    }
}
