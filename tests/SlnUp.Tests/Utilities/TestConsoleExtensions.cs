namespace SlnUp.Tests.Utilities;

using System.CommandLine;

internal static class TestConsoleExtensions
{
    public static string GetErrorOutput(this IConsole console) => console.Error.ToString()!;

    public static string GetOutput(this IConsole console) => console.Out.ToString()!;

    public static ConsoleAssertions Should(this IConsole console) => new(console);
}
