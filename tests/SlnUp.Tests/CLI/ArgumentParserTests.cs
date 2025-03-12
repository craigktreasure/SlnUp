namespace SlnUp.Tests.CLI;

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
        Option<Version?> versionOption = new("version") { CustomParser = ArgumentParser.ParseVersion };
        RootCommand rootCommand = [versionOption];
        Version expectedVersion = Version.Parse(expectedVersionValue);

        // Act
        ParseResult result = rootCommand.Parse(["version", expectedVersionValue]);

        // Assert
        await Assert.That(result.Errors).IsEmpty();
        Version? version = result.GetValue(versionOption);
        await Assert.That(version).IsNotNull();
        await Assert.That(version).IsEqualTo(expectedVersion);
    }

    [Test]
    public async Task ParseVersion_Invalid()
    {
        // Arrange
        Option<Version?> versionOption = new("version") { CustomParser = ArgumentParser.ParseVersion };
        RootCommand rootCommand = [versionOption];

        // Act
        ParseResult result = rootCommand.Parse(["version", "not-valid"]);

        // Assert
        await Assert.That(result.Errors).HasSingleItem();
        ParseError error = result.Errors.Single();
        await Assert.That(error.SymbolResult).IsNotNull();
        OptionResult optionResult = (await Assert.That(error.SymbolResult).IsTypeOf<OptionResult>())!;
        await Assert.That(optionResult.Option).IsEqualTo(versionOption);
    }
}
