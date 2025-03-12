namespace SlnUp.Tests.CLI;

using System;
using System.CommandLine;
using System.CommandLine.Parsing;

using SlnUp.CLI;

public class ArgumentParserTests
{
    [Test]
    [Arguments("1.2")]
    [Arguments("1.2.3")]
    [Arguments("1.2.3.4")]
    public async Task ParseVersion(string expectedVersionValue)
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
        await Assert.That(result.Errors).IsEmpty();
        Version? version = result.RootCommandResult.GetValueForOption(versionOption);
        await Assert.That(version).IsNotNull();
        await Assert.That(version).IsEqualTo(expectedVersion);
    }

    [Test]
    public async Task ParseVersion_Invalid()
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
        await Assert.That(result.Errors).HasSingleItem();
        ParseError error = result.Errors.Single();
        await Assert.That(error.SymbolResult?.Symbol.Name).IsEqualTo("version");
        await Assert.That(error.Message).StartsWith("Cannot parse argument 'not-valid'", StringComparison.Ordinal);
    }
}
