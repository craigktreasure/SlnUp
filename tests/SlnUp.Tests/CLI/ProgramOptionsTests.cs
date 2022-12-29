namespace SlnUp.Tests.CLI;

using System.CommandLine;
using System.CommandLine.IO;
using System.IO.Abstractions.TestingHelpers;

using SlnUp.CLI;
using SlnUp.TestLibrary.Extensions;
using SlnUp.Tests.Utilities;

public class ProgramOptionsTests
{
    private readonly TestConsole testConsole = new();

    [Theory]
    [InlineData("2022")]
    [InlineData("17.0")]
    public void Configure(string version)
    {
        // Arrange
        string[] args = new[] { version };

        // Act
        ProgramOptions? result = this.ConfigureAndInvoke(args, out int exitCode);

        // Assert
        Assert.Equal(0, exitCode);
        Assert.NotNull(result);
        result.Should().BeEquivalentTo(new ProgramOptions
        {
            Version = version
        });
    }

    [Fact]
    public void Configure_NoParameters()
    {
        // Arrange
        string[] args = Array.Empty<string>();

        // Act
        ProgramOptions? result = this.ConfigureAndInvoke(args, out int exitCode);

        // Assert
        Assert.Equal(0, exitCode);
        Assert.NotNull(result);
        result.Should().BeEquivalentTo(new ProgramOptions
        {
            Version = ProgramOptions.DefaultVersionArgument,
        });
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
        ProgramOptions? result = this.ConfigureAndInvoke(args, out int exitCode);

        // Assert
        Assert.Equal(0, exitCode);
        Assert.NotNull(result);
        result.Should().BeEquivalentTo(new ProgramOptions
        {
            Version = expectedVersion,
            BuildVersion = Version.Parse(expectedBuildVersion),
        });
    }

    [Fact]
    public void Configure_WithInvalidBuildVersion()
    {
        // Arrange
        string[] args = new[]
        {
            "2022",
            "--build-version",
            "invalid-version"
        };

        // Act
        ProgramOptions? result = this.ConfigureAndInvoke(args, out int exitCode);

        // Assert
        Assert.Equal(1, exitCode);
        Assert.Null(result);
        this.testConsole.Should().HaveOutputWritten();
        this.testConsole.Should().HaveErrorWritten();
        this.testConsole.GetErrorOutput().TrimEnd().Should().Be("Cannot parse argument 'invalid-version' for option 'build-version' as expected type 'System.Version'.");
    }

    [Theory]
    [InlineData("--help")]
    [InlineData("-h")]
    [InlineData("-?")]
    public void Configure_WithHelp(params string[] args)
    {
        // Act
        ProgramOptions? result = this.ConfigureAndInvoke(args, out int exitCode);

        // Assert
        Assert.Equal(0, exitCode);
        Assert.Null(result);
        this.testConsole.GetOutput().Should().StartWith("Description:");
        this.testConsole.Should().NotHaveErrorWritten();
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
        ProgramOptions? result = this.ConfigureAndInvoke(args.ToCrossPlatformPath().ToArray(), out int exitCode);

        // Assert
        Assert.Equal(0, exitCode);
        Assert.NotNull(result);
        result.Should().BeEquivalentTo(new ProgramOptions
        {
            Version = expectedVersion,
            SolutionPath = expectedFilePath,
        });
    }

    [Fact]
    public void Configure_WithVersion()
    {
        // Arrange
        string[] args = new[] { "--version" };

        // Act
        ProgramOptions? result = this.ConfigureAndInvoke(args, out int exitCode);

        // Assert
        Assert.Equal(0, exitCode);
        Assert.Null(result);
        this.testConsole.Should().HaveOutputWritten();
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
        result.Should().BeTrue();
        options.Should().NotBeNull();
        options!.SolutionFilePath.Should().Be(expectedSolutionFilePath);
        options.Version.Should().Be(Version.Parse("16.8.6"));
        options.BuildVersion.Should().Be(Version.Parse("16.8.31019.35"));
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
        result.Should().BeTrue();
        options.Should().NotBeNull();
        options!.SolutionFilePath.Should().Be(expectedSolutionFilePath);
        options.Version.Should().Be(Version.Parse("0.0"));
        options.BuildVersion.Should().Be(Version.Parse("0.0.0.0"));
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
        result.Should().BeFalse();
        options.Should().BeNull();
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
        result.Should().BeFalse();
        options.Should().BeNull();
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
        result.Should().BeFalse();
        options.Should().BeNull();
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
        result.Should().BeFalse();
        options.Should().BeNull();
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
        result.Should().BeFalse();
        options.Should().BeNull();
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

    private ProgramOptions? ConfigureAndInvoke(string[] args, out int exitCode)
    {
        ProgramOptions? options = null;

        exitCode = ProgramOptions.Configure(opts =>
        {
            options = opts;
            return 0;
        }).Invoke(args, this.testConsole);

        return options;
    }
}
