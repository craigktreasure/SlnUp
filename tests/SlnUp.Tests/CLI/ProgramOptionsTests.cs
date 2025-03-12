namespace SlnUp.Tests.CLI;

using System.CommandLine;
using System.IO.Abstractions.TestingHelpers;

using SlnUp.CLI;
using SlnUp.TestLibrary.Extensions;

public class ProgramOptionsTests
{
    [Test]
    [Arguments("2022")]
    [Arguments("2026")]
    [Arguments("17.0")]
    public async Task Configure(string version)
    {
        // Arrange
        string[] args = [version];

        // Act
        ProgramOptions? result = ConfigureAndInvoke(args, out int exitCode);

        // Assert
        await Assert.That(exitCode).IsEqualTo(0);
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Version).IsEqualTo(version);
    }

    [Test]
    public async Task Configure_NoParameters()
    {
        // Arrange
        string[] args = [];

        // Act
        ProgramOptions? result = ConfigureAndInvoke(args, out int exitCode);

        // Assert
        await Assert.That(exitCode).IsEqualTo(0);
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Version).IsEqualTo(ProgramOptions.DefaultVersionArgument);
    }

    [Test]
    [Arguments("2022", "--build-version", "17.0.31903.59")]
    [Arguments("--build-version", "17.0.31903.59", "2022")]
    public async Task Configure_WithBuildVersion(params string[] args)
    {
        // Arrange
        const string expectedVersion = "2022";
        const string expectedBuildVersion = "17.0.31903.59";

        // Act
        ProgramOptions? result = ConfigureAndInvoke(args, out int exitCode);

        // Assert
        await Assert.That(exitCode).IsEqualTo(0);
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Version).IsEqualTo(expectedVersion);
        await Assert.That(result.BuildVersion).IsEqualTo(Version.Parse(expectedBuildVersion));
    }

    [Test]
    [Arguments("--help")]
    [Arguments("-h")]
    [Arguments("-?")]
    public async Task Configure_WithHelp(params string[] args)
    {
        // Act
        ProgramOptions? result = ConfigureAndInvoke(args, out int exitCode, out string output, out string error);

        // Assert
        await Assert.That(exitCode).IsEqualTo(0);
        await Assert.That(result).IsNull();
        await Assert.That(output).StartsWith("Description:", StringComparison.Ordinal);
        await Assert.That(error).IsEmpty();
    }

    [Test]
    public async Task Configure_WithInvalidBuildVersion()
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
        await Assert.That(exitCode).IsEqualTo(1);
        await Assert.That(result).IsNull();
        await Assert.That(output).IsNotEmpty();
        await Assert.That(error).IsNotEmpty();
        await Assert.That(error.TrimEnd()).IsEqualTo(expectedErrorOutput);
    }

    [Test]
    [Arguments("2022", "--path", "C:/solution.sln")]
    [Arguments("2022", "-p", "C:/solution.sln")]
    [Arguments("-p", "C:/solution.sln", "2022")]
    public async Task Configure_WithPath(params string[] args)
    {
        // Arrange
        const string expectedVersion = "2022";
        string expectedFilePath = "C:/solution.sln".ToCrossPlatformPath();

        // Act
        ProgramOptions? result = ConfigureAndInvoke([.. args.ToCrossPlatformPath()], out int exitCode);

        // Assert
        await Assert.That(exitCode).IsEqualTo(0);
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Version).IsEqualTo(expectedVersion);
        await Assert.That(result.Path).IsEqualTo(expectedFilePath);
    }

    [Test]
    public async Task Configure_WithVersion()
    {
        // Arrange
        string[] args = ["--version"];

        // Act
        ProgramOptions? result = ConfigureAndInvoke(args, out int exitCode, out string output, out _);

        // Assert
        await Assert.That(exitCode).IsEqualTo(0);
        await Assert.That(result).IsNull();
        await Assert.That(output).IsNotEmpty();
    }

    /// <summary>
    /// Minimal invocation: > app 16.8
    /// </summary>
    [Test]
    public async Task TryGetSlnUpOptions()
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
        await Assert.That(result).IsTrue();
        await Assert.That(options).IsNotNull();
        await Assert.That(options!.SolutionFilePath).IsEqualTo(expectedSolutionFilePath);
        await Assert.That(options.Version).IsEqualTo(Version.Parse("16.8.7"));
        await Assert.That(options.BuildVersion).IsEqualTo(Version.Parse("16.8.31025.109"));
    }

    /// <summary>
    /// app 16.8 --path C:\MyProject.sln
    /// </summary>
    [Test]
    public async Task TryGetSlnUpOptions_WithAbsoluteSolutionPath()
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
        await Assert.That(result).IsTrue();
        await Assert.That(options).IsNotNull();
        await Assert.That(options!.SolutionFilePath).IsEqualTo(expectedSolutionFilePath);
    }

    /// <summary>
    /// Build version: > app 0.0 --build-version 0.0.0.0
    /// </summary>
    [Test]
    public async Task TryGetSlnUpOptions_WithBuildVersion()
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
        await Assert.That(result).IsTrue();
        await Assert.That(options).IsNotNull();
        await Assert.That(options!.SolutionFilePath).IsEqualTo(expectedSolutionFilePath);
        await Assert.That(options.Version).IsEqualTo(Version.Parse("0.0"));
        await Assert.That(options.BuildVersion).IsEqualTo(Version.Parse("0.0.0.0"));
    }

    /// <summary>
    /// Build version with invalid version: > app 0 --build-version 0.0.0
    /// </summary>
    [Test]
    public async Task TryGetSlnUpOptions_WithBuildVersionAndInvalidVersion()
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
        await Assert.That(result).IsFalse();
        await Assert.That(options).IsNull();
    }

    /// <summary>
    /// Invalid build version: > app 0.0 --build-version 0.0.0
    /// </summary>
    [Test]
    public async Task TryGetSlnUpOptions_WithInvalidBuildVersion()
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
        await Assert.That(result).IsFalse();
        await Assert.That(options).IsNull();
    }

    /// <summary>
    /// Minimal invocation: > app 0.0
    /// </summary>
    [Test]
    public async Task TryGetSlnUpOptions_WithInvalidVersion()
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
        await Assert.That(result).IsFalse();
        await Assert.That(options).IsNull();
    }

    /// <summary>
    /// Multiple solution files available: > app 16.8
    /// </summary>
    [Test]
    public async Task TryGetSlnUpOptions_WithMultipleSolutions()
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
        await Assert.That(result).IsFalse();
        await Assert.That(options).IsNull();
    }

    /// <summary>
    /// app 16.8 --path C:\Missing.sln
    /// </summary>
    [Test]
    public async Task TryGetSlnUpOptions_WithNonExistentSolutionPath()
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
        await Assert.That(result).IsFalse();
        await Assert.That(options).IsNull();
    }

    /// <summary>
    /// Multiple solution files available: > app 16.8
    /// </summary>
    [Test]
    public async Task TryGetSlnUpOptions_WithNoSolutions()
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
        await Assert.That(result).IsFalse();
        await Assert.That(options).IsNull();
    }

    /// <summary>
    /// app 16.8 --path .\MyProject.sln
    /// </summary>
    [Test]
    public async Task TryGetSlnUpOptions_WithRelativeSolutionPath()
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
        await Assert.That(result).IsTrue();
        await Assert.That(options).IsNotNull();
        await Assert.That(options!.SolutionFilePath).IsEqualTo(expectedSolutionFilePath);
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
