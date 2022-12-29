namespace SlnUp.Tests.Utilities;

using System.CommandLine;

using FluentAssertions.Primitives;

internal sealed class ConsoleAssertions : ReferenceTypeAssertions<IConsole, ConsoleAssertions>
{
    protected override string Identifier => nameof(IConsole);

    public ConsoleAssertions(IConsole console)
            : base(console)
    {
    }

    public void HaveErrorWritten()
        => this.Subject.GetErrorOutput().Should().NotBeNullOrEmpty();

    public void HaveOutputWritten()
        => this.Subject.GetOutput().Should().NotBeNullOrEmpty();

    public void NotHaveErrorWritten()
        => this.Subject.GetErrorOutput().Should().BeNullOrEmpty();

    public void NotHaveOutputWritten()
        => this.Subject.GetOutput().Should().BeNullOrEmpty();
}
