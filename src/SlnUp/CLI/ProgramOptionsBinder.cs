namespace SlnUp.CLI;

using System.CommandLine;
using System.CommandLine.Binding;

internal class ProgramOptionsBinder : BinderBase<ProgramOptions>
{
    private readonly Option<Version?> buildVersionOption;

    private readonly Option<string?> pathOption;

    private readonly Argument<string?> versionArgument;

    public ProgramOptionsBinder(
        Option<string?> pathOption,
        Argument<string?> versionArgument,
        Option<Version?> buildVersionOption)
    {
        ArgumentNullException.ThrowIfNull(pathOption);
        ArgumentNullException.ThrowIfNull(versionArgument);
        ArgumentNullException.ThrowIfNull(buildVersionOption);

        this.pathOption = pathOption;
        this.versionArgument = versionArgument;
        this.buildVersionOption = buildVersionOption;
    }

    protected override ProgramOptions GetBoundValue(BindingContext bindingContext) => new()
    {
        BuildVersion = bindingContext.ParseResult.GetValueForOption(this.buildVersionOption),
        SolutionPath = bindingContext.ParseResult.GetValueForOption(this.pathOption),
        Version = bindingContext.ParseResult.GetValueForArgument(this.versionArgument),
    };
}
