namespace SlnUp.Tests.CLI;

using System;
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
        Option<Version?> versionOption = new("--version", ArgumentParser.ParseVersion);
        Parser parser = new(new RootCommand()
        {
            versionOption,
        });
        Version expectedVersion = Version.Parse(expectedVersionValue);

        // Act
        ParseResult result = parser.Parse($"--version {expectedVersionValue}");

        // Assert
        Assert.Empty(result.Errors);
        Version? version = result.RootCommandResult.GetValueForOption(versionOption);
        Assert.NotNull(version);
        Assert.Equal(expectedVersion, version);
    }

    [Fact]
    public void ParseVersion_Invalid()
    {
        // Arrange
        Option<Version?> versionOption = new("--version", ArgumentParser.ParseVersion);
        Parser parser = new(new RootCommand()
        {
            versionOption,
        });

        // Act
        ParseResult result = parser.Parse("--version not-valid");

        // Assert
        ParseError error = Assert.Single(result.Errors);
        Assert.Equal("version", error.SymbolResult?.Symbol.Name);
        error.Message.Should().StartWith("Cannot parse argument 'not-valid'");
    }
}
