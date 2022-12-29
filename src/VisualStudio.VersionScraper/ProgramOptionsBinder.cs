namespace VisualStudio.VersionScraper;

using System.CommandLine;
using System.CommandLine.Binding;

internal sealed class ProgramOptionsBinder : BinderBase<ProgramOptions>
{
    private readonly Option<bool> noCacheOption;

    private readonly Argument<string?> outputArgument;

    private readonly Option<OutputFormat> formatOption;

    public ProgramOptionsBinder(
        Argument<string?> outputArgument,
        Option<OutputFormat> formatOption,
        Option<bool> noCacheOption)
    {
        ArgumentNullException.ThrowIfNull(outputArgument);
        ArgumentNullException.ThrowIfNull(formatOption);
        ArgumentNullException.ThrowIfNull(noCacheOption);

        this.outputArgument = outputArgument;
        this.formatOption = formatOption;
        this.noCacheOption = noCacheOption;
    }

    protected override ProgramOptions GetBoundValue(BindingContext bindingContext) => new()
    {
        OutputFilePath = bindingContext.ParseResult.GetValueForArgument(this.outputArgument),
        Format = bindingContext.ParseResult.GetValueForOption(this.formatOption),
        NoCache = bindingContext.ParseResult.GetValueForOption(this.noCacheOption),
    };
}
