namespace SlnUp.Tests.Utilities;

using System.CommandLine;

internal static class TestConsoleExtensions
{
    public static string GetErrorOutput(this IConsole console) => console.Error.ToString() ?? string.Empty;

    public static string GetOutput(this IConsole console) => console.Out.ToString() ?? string.Empty;

    public static bool HasErrorOutput(this IConsole console) => !string.IsNullOrEmpty(console.GetErrorOutput());

    public static bool HasNoErrorOutput(this IConsole console) => !console.HasErrorOutput();

    public static bool HasNoOutput(this IConsole console) => !console.HasOutput();

    public static bool HasOutput(this IConsole console) => !string.IsNullOrEmpty(console.GetOutput());
}
