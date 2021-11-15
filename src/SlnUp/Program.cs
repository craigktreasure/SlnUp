namespace SlnUp;

using CommandLine;

internal static class Program
{
    public static int Main(string[] args)
    {
        Console.Title = "Visual Studio Solution Updater";

        return ProgramOptions.ParseOptions(args).MapResult(Run, _ => 1);
    }

    private static int Run(ProgramOptions options)
    {
        return 0;
    }
}
