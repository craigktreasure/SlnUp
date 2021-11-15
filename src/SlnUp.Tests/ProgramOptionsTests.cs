namespace SlnUp.Tests;

using CommandLine;
using FluentAssertions;
using Xunit;

public class ProgramOptionsTests
{
    [Theory]
    [InlineData("2022")]
    [InlineData("17.0")]
    public void ParseOptions(string version)
    {
        // Arrange
        string[] args = new[] { version };

        // Act
        ParserResult<ProgramOptions> parseResult = ProgramOptions.ParseOptions(args);

        // Assert
        Parsed<ProgramOptions> result = parseResult.Should().BeAssignableTo<Parsed<ProgramOptions>>().Subject;
        result.Value.Should().BeEquivalentTo(new ProgramOptions
        {
            Version = version
        });
    }

    [Fact]
    public void ParseOptionsNoParameters()
    {
        // Arrange
        string[] args = Array.Empty<string>();
        StringWriter helpWriter = new();
        Parser parser = new(config => config.HelpWriter = helpWriter);

        // Act
        ParserResult<ProgramOptions> parseResult = ProgramOptions.ParseOptions(args, parser);
        string helpContent = helpWriter.ToString();

        // Assert
        NotParsed<ProgramOptions> result = parseResult.Should().BeAssignableTo<NotParsed<ProgramOptions>>().Subject;
        Error error = result.Errors.Should().ContainSingle().Subject;
        error.Tag.Should().Be(ErrorType.MissingRequiredOptionError);
        helpContent.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("2022", "--build-version", "17.0.31903.59")]
    [InlineData("--build-version", "17.0.31903.59", "2022")]
    public void ParseOptionsWithBuildVersion(params string[] args)
    {
        // Arrange
        const string expectedVersion = "2022";
        const string expectedBuildVersion = "17.0.31903.59";

        // Act
        ParserResult<ProgramOptions> parseResult = ProgramOptions.ParseOptions(args);

        // Assert
        Parsed<ProgramOptions> result = parseResult.Should().BeAssignableTo<Parsed<ProgramOptions>>().Subject;
        result.Value.Should().BeEquivalentTo(new ProgramOptions
        {
            Version = expectedVersion,
            BuildVersion = Version.Parse(expectedBuildVersion),
        });
    }

    [Fact]
    public void ParseOptionsWithHelp()
    {
        // Arrange
        string[] args = new[] { "--help" };
        StringWriter helpWriter = new();
        Parser parser = new(config => config.HelpWriter = helpWriter);

        // Act
        ParserResult<ProgramOptions> parseResult = ProgramOptions.ParseOptions(args, parser);
        string helpContent = helpWriter.ToString();

        // Assert
        NotParsed<ProgramOptions> result = parseResult.Should().BeAssignableTo<NotParsed<ProgramOptions>>().Subject;
        Error error = result.Errors.Should().ContainSingle().Subject;
        error.StopsProcessing.Should().BeTrue();
        error.Tag.Should().Be(ErrorType.HelpRequestedError);
        helpContent.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("2022", "--path", "C:/solution.sln")]
    [InlineData("2022", "-p", "C:/solution.sln")]
    [InlineData("-p", "C:/solution.sln", "2022")]
    public void ParseOptionsWithPath(params string[] args)
    {
        // Arrange
        const string expectedVersion = "2022";
        const string expectedFilePath = "C:/solution.sln";

        // Act
        ParserResult<ProgramOptions> parseResult = ProgramOptions.ParseOptions(args);

        // Assert
        Parsed<ProgramOptions> result = parseResult.Should().BeAssignableTo<Parsed<ProgramOptions>>().Subject;
        result.Value.Should().BeEquivalentTo(new ProgramOptions
        {
            Version = expectedVersion,
            SolutionPath = expectedFilePath,
        });
    }

    [Fact]
    public void ParseOptionsWithVersion()
    {
        // Arrange
        string[] args = new[] { "--version" };

        // Act
        ParserResult<ProgramOptions> parseResult = ProgramOptions.ParseOptions(args);

        // Assert
        NotParsed<ProgramOptions> result = parseResult.Should().BeAssignableTo<NotParsed<ProgramOptions>>().Subject;
        Error error = result.Errors.Should().ContainSingle().Subject;
        error.StopsProcessing.Should().BeTrue();
        error.Tag.Should().Be(ErrorType.VersionRequestedError);
    }
}
