namespace SlnUp.Tests;

using CommandLine;
using FluentAssertions;
using SlnUp.TestLibrary;
using System.IO.Abstractions.TestingHelpers;
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
        using StringWriter helpWriter = new();
        using Parser parser = new(config => config.HelpWriter = helpWriter);

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
        using StringWriter helpWriter = new();
        using Parser parser = new(config => config.HelpWriter = helpWriter);

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
        string expectedFilePath = "C:/solution.sln".ToCrossPlatformPath();

        // Act
        ParserResult<ProgramOptions> parseResult = ProgramOptions.ParseOptions(args.ToCrossPlatformPath().ToArray());

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

    // Minimal invocation: > app 16.8
    [Fact]
    public void TryGetSlnUpOptions()
    {
        // Arrange
        string expectedSolutionFilePath = "C:\\MyProject.sln".ToCrossPlatformPath();
        ProgramOptions programOptions = new()
        {
            Version = "16.8"
        };
        MockFileSystem fileSystem = new(new Dictionary<string, MockFileData>
        {
            [expectedSolutionFilePath] = new MockFileData(string.Empty),
        }, "C:\\".ToCrossPlatformPath());

        // Act
        bool result = programOptions.TryGetSlnUpOptions(fileSystem, out SlnUpOptions? options);

        // Assert
        result.Should().BeTrue();
        options.Should().NotBeNull();
        options!.SolutionFilePath.Should().Be(expectedSolutionFilePath);
        options.Version.Should().Be(Version.Parse("16.8.6"));
        options.BuildVersion.Should().Be(Version.Parse("16.8.31019.35"));
    }

    // Build version: > app 0.0 --build-version 0.0.0.0
    [Fact]
    public void TryGetSlnUpOptionsWithBuildVersion()
    {
        // Arrange
        string expectedSolutionFilePath = "C:\\MyProject.sln".ToCrossPlatformPath();
        ProgramOptions programOptions = new()
        {
            Version = "0.0",
            BuildVersion = Version.Parse("0.0.0.0")
        };
        MockFileSystem fileSystem = new(new Dictionary<string, MockFileData>
        {
            [expectedSolutionFilePath] = new MockFileData(string.Empty),
        }, "C:\\".ToCrossPlatformPath());

        // Act
        bool result = programOptions.TryGetSlnUpOptions(fileSystem, out SlnUpOptions? options);

        // Assert
        result.Should().BeTrue();
        options.Should().NotBeNull();
        options!.SolutionFilePath.Should().Be(expectedSolutionFilePath);
        options.Version.Should().Be(Version.Parse("0.0"));
        options.BuildVersion.Should().Be(Version.Parse("0.0.0.0"));
    }

    // Invalid build version: > app 0.0 --build-version 0.0.0
    [Fact]
    public void TryGetSlnUpOptionsWithInvalidBuildVersion()
    {
        // Arrange
        string expectedSolutionFilePath = "C:\\MyProject.sln".ToCrossPlatformPath();
        ProgramOptions programOptions = new()
        {
            Version = "0.0",
            BuildVersion = Version.Parse("0.0.0")
        };
        MockFileSystem fileSystem = new(new Dictionary<string, MockFileData>
        {
            [expectedSolutionFilePath] = new MockFileData(string.Empty),
        }, "C:\\".ToCrossPlatformPath());

        // Act
        bool result = programOptions.TryGetSlnUpOptions(fileSystem, out SlnUpOptions? options);

        // Assert
        result.Should().BeFalse();
        options.Should().BeNull();
    }

    // Minimal invocation: > app 0.0
    [Fact]
    public void TryGetSlnUpOptionsWithInvalidVersion()
    {
        // Arrange
        string expectedSolutionFilePath = "C:\\MyProject.sln".ToCrossPlatformPath();
        ProgramOptions programOptions = new()
        {
            Version = "0.0"
        };
        MockFileSystem fileSystem = new(new Dictionary<string, MockFileData>
        {
            [expectedSolutionFilePath] = new MockFileData(string.Empty),
        }, "C:\\".ToCrossPlatformPath());

        // Act
        bool result = programOptions.TryGetSlnUpOptions(fileSystem, out SlnUpOptions? options);

        // Assert
        result.Should().BeFalse();
        options.Should().BeNull();
    }

    // app 16.8 --path C:\MyProject.sln
    [Fact]
    public void TryGetSlnUpOptionsWithAbsoluteSolutionPath()
    {
        // Arrange
        string expectedSolutionFilePath = "C:\\MyProject.sln".ToCrossPlatformPath();
        ProgramOptions programOptions = new()
        {
            Version = "16.8",
            SolutionPath = expectedSolutionFilePath
        };
        MockFileSystem fileSystem = new(new Dictionary<string, MockFileData>
        {
            [expectedSolutionFilePath] = new MockFileData(string.Empty),
        }, "C:\\".ToCrossPlatformPath());

        // Act
        bool result = programOptions.TryGetSlnUpOptions(fileSystem, out SlnUpOptions? options);

        // Assert
        result.Should().BeTrue();
        options.Should().NotBeNull();
        options!.SolutionFilePath.Should().Be(expectedSolutionFilePath);
    }

    // Multiple solution files available: > app 16.8
    [Fact]
    public void TryGetSlnUpOptionsWithMultipleSolutions()
    {
        // Arrange
        ProgramOptions programOptions = new()
        {
            Version = "16.8"
        };
        MockFileSystem fileSystem = new(new Dictionary<string, MockFileData>
        {
            ["C:\\MyProject.sln".ToCrossPlatformPath()] = new MockFileData(string.Empty),
            ["C:\\MyProject2.sln".ToCrossPlatformPath()] = new MockFileData(string.Empty),
        }, "C:\\".ToCrossPlatformPath());

        // Act
        bool result = programOptions.TryGetSlnUpOptions(fileSystem, out SlnUpOptions? options);

        // Assert
        result.Should().BeFalse();
        options.Should().BeNull();
    }

    // app 16.8 --path C:\Missing.sln
    [Fact]
    public void TryGetSlnUpOptionsWithNonExistentSolutionPath()
    {
        // Arrange
        ProgramOptions programOptions = new()
        {
            Version = "16.8",
            SolutionPath = "C:\\Missing.sln".ToCrossPlatformPath(),
        };
        MockFileSystem fileSystem = new(new Dictionary<string, MockFileData>
        {
            ["C:\\MyProject.sln".ToCrossPlatformPath()] = new MockFileData(string.Empty),
        }, "C:\\".ToCrossPlatformPath());

        // Act
        bool result = programOptions.TryGetSlnUpOptions(fileSystem, out SlnUpOptions? options);

        // Assert
        result.Should().BeFalse();
        options.Should().BeNull();
    }

    // Multiple solution files available: > app 16.8
    [Fact]
    public void TryGetSlnUpOptionsWithNoSolutions()
    {
        // Arrange
        ProgramOptions programOptions = new()
        {
            Version = "16.8"
        };
        MockFileSystem fileSystem = new();

        // Act
        bool result = programOptions.TryGetSlnUpOptions(fileSystem, out SlnUpOptions? options);

        // Assert
        result.Should().BeFalse();
        options.Should().BeNull();
    }

    // app 16.8 --path .\MyProject.sln
    [Fact]
    public void TryGetSlnUpOptionsWithRelativeSolutionPath()
    {
        // Arrange
        string expectedSolutionFilePath = "C:\\MyProject.sln".ToCrossPlatformPath();
        ProgramOptions programOptions = new()
        {
            Version = "16.8",
            SolutionPath = ".\\MyProject.sln".ToCrossPlatformPath(),
        };
        MockFileSystem fileSystem = new(new Dictionary<string, MockFileData>
        {
            [expectedSolutionFilePath] = new MockFileData(string.Empty),
        }, "C:\\".ToCrossPlatformPath());

        // Act
        bool result = programOptions.TryGetSlnUpOptions(fileSystem, out SlnUpOptions? options);

        // Assert
        result.Should().BeTrue();
        options.Should().NotBeNull();
        options!.SolutionFilePath.Should().Be(expectedSolutionFilePath);
    }
}
