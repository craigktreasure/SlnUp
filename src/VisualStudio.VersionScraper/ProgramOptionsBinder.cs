namespace VisualStudio.VersionScraper;

using System.CommandLine;
using System.CommandLine.Binding;

internal class ProgramOptionsBinder : BinderBase<ProgramOptions>
{
    private readonly Option<bool> noCacheOption;

    private readonly Argument<string?> outputArgument;

    public ProgramOptionsBinder(
        Argument<string?> outputArgument,
        Option<bool> noCacheOption)
    {
        ArgumentNullException.ThrowIfNull(outputArgument);
        ArgumentNullException.ThrowIfNull(noCacheOption);

        this.outputArgument = outputArgument;
        this.noCacheOption = noCacheOption;
    }

    protected override ProgramOptions GetBoundValue(BindingContext bindingContext) => new()
    {
        OutputFilePath = bindingContext.ParseResult.GetValueForArgument(this.outputArgument),
        NoCache = bindingContext.ParseResult.GetValueForOption(this.noCacheOption),
    };
}
