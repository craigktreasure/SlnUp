namespace SlnUp.Tests.CLI;

using System.CommandLine;
using System.CommandLine.IO;
using System.Text;

using SlnUp.CLI;
using SlnUp.Tests.Utilities;

public class ProgramOptionsBinderTests
{
    private readonly TestConsole testConsole = new();

    [Fact]
    public void Constructor()
    {
        // Arrange
        Option<string?> pathOption = new("--path");
        Argument<string?> versionArgument = new("version");
        Option<Version?> buildVersionOption = new("--build-version");

        // Act and assert
        _ = new ProgramOptionsBinder(pathOption, versionArgument, buildVersionOption);
        Assert.Throws<ArgumentNullException>(nameof(pathOption),
            () => new ProgramOptionsBinder(null!, versionArgument, buildVersionOption));
        Assert.Throws<ArgumentNullException>(nameof(versionArgument),
            () => new ProgramOptionsBinder(pathOption, null!, buildVersionOption));
        Assert.Throws<ArgumentNullException>(nameof(buildVersionOption),
            () => new ProgramOptionsBinder(pathOption, versionArgument, null!));
    }

    [Theory]
    [InlineData(null, null, null)]
    [InlineData("2022", null, null)]
    [InlineData("2022", null, "17.2.0")]
    [InlineData("2022", "C:/path.sln", null)]
    [InlineData("2022", "C:/path.sln", "17.2.0")]
    public void GetBoundValue(string? version, string? expectedPath, string? expectedBuildVersion)
    {
        // Arrange
        string expectedVersion = version ?? ProgramOptions.DefaultVersionArgument;
        Option<string?> pathOption = new("--path");
        Argument<string?> versionArgument = new("version", getDefaultValue: () => ProgramOptions.DefaultVersionArgument);
        Option<Version?> buildVersionOption = new("--build-version", parseArgument: ArgumentParser.ParseVersion);
        RootCommand command = new()
        {
            pathOption,
            versionArgument,
            buildVersionOption,
        };
        ProgramOptionsBinder binder = new(pathOption, versionArgument, buildVersionOption);
        StringBuilder argsBuilder = new(version);

        if (expectedPath is not null)
        {
            argsBuilder.Append(" --path ").Append(expectedPath);
        }

        if (expectedBuildVersion is not null)
        {
            argsBuilder.Append(" --build-version ").Append(expectedBuildVersion);
        }

        // Act
        ProgramOptions? options = null;
        command.SetHandler(opts => options = opts, binder);
        command.Invoke(argsBuilder.ToString(), this.testConsole);

        // Assert
        this.testConsole.Should().NotHaveErrorWritten();
        this.testConsole.Should().NotHaveOutputWritten();
        Assert.NotNull(options);
        Assert.Equal(expectedVersion, options.Version);
        Assert.Equal(expectedPath, options.SolutionPath);
#pragma warning disable CA1508 // Avoid dead conditional code
        Assert.Equal(expectedBuildVersion, options.BuildVersion?.ToString());
#pragma warning restore CA1508 // Avoid dead conditional code
    }
}
