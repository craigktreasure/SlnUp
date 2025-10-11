namespace SlnUp.Tests.CLI;

using System.CommandLine;
using System.CommandLine.Parsing;

using SlnUp.CLI;

public class ArgumentParserTests
{
    [Theory]
    [InlineData("1.2")]
    [InlineData("1.2.3")]
    [InlineData("1.2.3.4")]
    public void ParseVersion(string expectedVersionValue)
    {
        // Arrange
        Option<Version?> versionOption = new("version") { CustomParser = ArgumentParser.ParseVersion };
        RootCommand rootCommand = [versionOption];
        Version expectedVersion = Version.Parse(expectedVersionValue);

        // Act
        ParseResult result = rootCommand.Parse(["version", expectedVersionValue]);

        // Assert
        Assert.Empty(result.Errors);
        Version? version = result.GetValue(versionOption);
        Assert.NotNull(version);
        Assert.Equal(expectedVersion, version);
    }

    [Fact]
    public void ParseVersion_Invalid()
    {
        // Arrange
        Option<Version?> versionOption = new("version") { CustomParser = ArgumentParser.ParseVersion };
        RootCommand rootCommand = [versionOption];

        // Act
        ParseResult result = rootCommand.Parse(["version", "not-valid"]);

        // Assert
        ParseError error = Assert.Single(result.Errors);
        OptionResult optionResult = Assert.IsType<OptionResult>(error.SymbolResult);
        Assert.Equal(versionOption, optionResult.Option);
        Assert.StartsWith("Cannot parse argument 'not-valid'", error.Message, StringComparison.Ordinal);
    }
}
