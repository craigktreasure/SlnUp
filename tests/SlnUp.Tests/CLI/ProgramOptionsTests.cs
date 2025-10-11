namespace SlnUp.Tests.CLI;

using System.CommandLine;
using System.IO.Abstractions.TestingHelpers;

using SlnUp.CLI;
using SlnUp.TestLibrary.Extensions;

public class ProgramOptionsTests
{
    [Theory]
    [InlineData("2022")]
    [InlineData("17.0")]
    public void Configure(string version)
    {
        // Arrange
        string[] args = [version];

        // Act
        ProgramOptions? result = ConfigureAndInvoke(args, out int exitCode);

        // Assert
        Assert.Equal(0, exitCode);
        Assert.NotNull(result);
        Assert.Equal(version, result.Version);
    }

    [Fact]
    public void Configure_NoParameters()
    {
        // Arrange
        string[] args = [];

        // Act
        ProgramOptions? result = ConfigureAndInvoke(args, out int exitCode);

        // Assert
        Assert.Equal(0, exitCode);
        Assert.NotNull(result);
        Assert.Equal(ProgramOptions.DefaultVersionArgument, result.Version);
    }

    [Theory]
    [InlineData("2022", "--build-version", "17.0.31903.59")]
    [InlineData("--build-version", "17.0.31903.59", "2022")]
    public void Configure_WithBuildVersion(params string[] args)
    {
        // Arrange
        const string expectedVersion = "2022";
        const string expectedBuildVersion = "17.0.31903.59";

        // Act
        ProgramOptions? result = ConfigureAndInvoke(args, out int exitCode);

        // Assert
        Assert.Equal(0, exitCode);
        Assert.NotNull(result);
        Assert.Equal(expectedVersion, result.Version);
        Assert.Equal(Version.Parse(expectedBuildVersion), result.BuildVersion);
    }

    [Theory]
    [InlineData("--help")]
    [InlineData("-h")]
    [InlineData("-?")]
    public void Configure_WithHelp(params string[] args)
    {
        // Act
        ProgramOptions? result = ConfigureAndInvoke(args, out int exitCode, out string output, out string error);

        // Assert
        Assert.Equal(0, exitCode);
        Assert.Null(result);
        Assert.StartsWith("Description:", output, StringComparison.Ordinal);
        Assert.Empty(error);
    }

    [Fact]
    public void Configure_WithInvalidBuildVersion()
    {
        // Arrange
        string[] args =
        [
            "2022",
            "--build-version",
            "invalid-version"
        ];
        const string expectedErrorOutput = "Cannot parse argument 'invalid-version' for option '--build-version' as expected type 'System.Version'.";

        // Act
        ProgramOptions? result = ConfigureAndInvoke(args, out int exitCode, out string output, out string error);

        // Assert
        Assert.Equal(1, exitCode);
        Assert.Null(result);
        Assert.NotEmpty(output);
        Assert.NotEmpty(error);
        Assert.Equal(expectedErrorOutput, error.TrimEnd());
    }

    [Theory]
    [InlineData("2022", "--path", "C:/solution.sln")]
    [InlineData("2022", "-p", "C:/solution.sln")]
    [InlineData("-p", "C:/solution.sln", "2022")]
    public void Configure_WithPath(params string[] args)
    {
        // Arrange
        const string expectedVersion = "2022";
        string expectedFilePath = "C:/solution.sln".ToCrossPlatformPath();

        // Act
        ProgramOptions? result = ConfigureAndInvoke([.. args.ToCrossPlatformPath()], out int exitCode);

        // Assert
        Assert.Equal(0, exitCode);
        Assert.NotNull(result);
        Assert.Equal(expectedVersion, result.Version);
        Assert.Equal(expectedFilePath, result.Path);
    }

    [Fact]
    public void Configure_WithVersion()
    {
        // Arrange
        string[] args = ["--version"];

        // Act
        ProgramOptions? result = ConfigureAndInvoke(args, out int exitCode, out string output, out _);

        // Assert
        Assert.Equal(0, exitCode);
        Assert.Null(result);
        Assert.NotEmpty(output);
    }

    /// <summary>
    /// Minimal invocation: > app 16.8
    /// </summary>
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
        Assert.True(result);
        Assert.NotNull(options);
        Assert.Equal(expectedSolutionFilePath, options.SolutionFilePath);
        Assert.Equal(Version.Parse("16.8.7"), options.Version);
        Assert.Equal(Version.Parse("16.8.31025.109"), options.BuildVersion);
    }

    /// <summary>
    /// app 16.8 --path C:\MyProject.sln
    /// </summary>
    [Fact]
    public void TryGetSlnUpOptions_WithAbsoluteSolutionPath()
    {
        // Arrange
        string expectedSolutionFilePath = "C:\\MyProject.sln".ToCrossPlatformPath();
        ProgramOptions programOptions = new()
        {
            Version = "16.8",
            Path = expectedSolutionFilePath
        };
        MockFileSystem fileSystem = new(new Dictionary<string, MockFileData>
        {
            [expectedSolutionFilePath] = new MockFileData(string.Empty),
        }, "C:\\".ToCrossPlatformPath());

        // Act
        bool result = programOptions.TryGetSlnUpOptions(fileSystem, out SlnUpOptions? options);

        // Assert
        Assert.True(result);
        Assert.NotNull(options);
        Assert.Equal(expectedSolutionFilePath, options.SolutionFilePath);
    }

    /// <summary>
    /// Build version: > app 0.0 --build-version 0.0.0.0
    /// </summary>
    [Fact]
    public void TryGetSlnUpOptions_WithBuildVersion()
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
        Assert.True(result);
        Assert.NotNull(options);
        Assert.Equal(expectedSolutionFilePath, options.SolutionFilePath);
        Assert.Equal(Version.Parse("0.0"), options.Version);
        Assert.Equal(Version.Parse("0.0.0.0"), options.BuildVersion);
    }

    /// <summary>
    /// Build version with invalid version: > app 0 --build-version 0.0.0
    /// </summary>
    [Fact]
    public void TryGetSlnUpOptions_WithBuildVersionAndInvalidVersion()
    {
        // Arrange
        string expectedSolutionFilePath = "C:\\MyProject.sln".ToCrossPlatformPath();
        ProgramOptions programOptions = new()
        {
            Version = "0",
            BuildVersion = Version.Parse("0.0.0.0")
        };
        MockFileSystem fileSystem = new(new Dictionary<string, MockFileData>
        {
            [expectedSolutionFilePath] = new MockFileData(string.Empty),
        }, "C:\\".ToCrossPlatformPath());

        // Act
        bool result = programOptions.TryGetSlnUpOptions(fileSystem, out SlnUpOptions? options);

        // Assert
        Assert.False(result);
        Assert.Null(options);
    }

    /// <summary>
    /// Invalid build version: > app 0.0 --build-version 0.0.0
    /// </summary>
    [Fact]
    public void TryGetSlnUpOptions_WithInvalidBuildVersion()
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
        Assert.False(result);
        Assert.Null(options);
    }

    /// <summary>
    /// Minimal invocation: > app 0.0
    /// </summary>
    [Fact]
    public void TryGetSlnUpOptions_WithInvalidVersion()
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
        Assert.False(result);
        Assert.Null(options);
    }

    /// <summary>
    /// Multiple solution files available: > app 16.8
    /// </summary>
    [Fact]
    public void TryGetSlnUpOptions_WithMultipleSolutions()
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
        Assert.False(result);
        Assert.Null(options);
    }

    /// <summary>
    /// app 16.8 --path C:\Missing.sln
    /// </summary>
    [Fact]
    public void TryGetSlnUpOptions_WithNonExistentSolutionPath()
    {
        // Arrange
        ProgramOptions programOptions = new()
        {
            Version = "16.8",
            Path = "C:\\Missing.sln".ToCrossPlatformPath(),
        };
        MockFileSystem fileSystem = new(new Dictionary<string, MockFileData>
        {
            ["C:\\MyProject.sln".ToCrossPlatformPath()] = new MockFileData(string.Empty),
        }, "C:\\".ToCrossPlatformPath());

        // Act
        bool result = programOptions.TryGetSlnUpOptions(fileSystem, out SlnUpOptions? options);

        // Assert
        Assert.False(result);
        Assert.Null(options);
    }

    /// <summary>
    /// Multiple solution files available: > app 16.8
    /// </summary>
    [Fact]
    public void TryGetSlnUpOptions_WithNoSolutions()
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
        Assert.False(result);
        Assert.Null(options);
    }

    /// <summary>
    /// app 16.8 --path .\MyProject.sln
    /// </summary>
    [Fact]
    public void TryGetSlnUpOptions_WithRelativeSolutionPath()
    {
        // Arrange
        string expectedSolutionFilePath = "C:\\MyProject.sln".ToCrossPlatformPath();
        ProgramOptions programOptions = new()
        {
            Version = "16.8",
            Path = ".\\MyProject.sln".ToCrossPlatformPath(),
        };
        MockFileSystem fileSystem = new(new Dictionary<string, MockFileData>
        {
            [expectedSolutionFilePath] = new MockFileData(string.Empty),
        }, "C:\\".ToCrossPlatformPath());

        // Act
        bool result = programOptions.TryGetSlnUpOptions(fileSystem, out SlnUpOptions? options);

        // Assert
        Assert.True(result);
        Assert.NotNull(options);
        Assert.Equal(expectedSolutionFilePath, options.SolutionFilePath);
    }

    private static ProgramOptions? ConfigureAndInvoke(string[] args, out int exitCode)
        => ConfigureAndInvoke(args, out exitCode, out _, out _);

    private static ProgramOptions? ConfigureAndInvoke(string[] args, out int exitCode, out string output, out string error)
    {
        ProgramOptions? options = null;

        RootCommand rootCommand = ProgramOptions.Configure(opts =>
        {
            options = opts;
            return 0;
        });

        ParseResult result = rootCommand.Parse(args);

        using StringWriter outputWriter = new();
        using StringWriter errorWriter = new();

        InvocationConfiguration invocationConfig = new()
        {
            Output = outputWriter,
            Error = errorWriter
        };

        exitCode = result.Invoke(invocationConfig);
        output = outputWriter.ToString();
        error = errorWriter.ToString();

        return options;
    }
}
