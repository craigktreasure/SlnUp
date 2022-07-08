namespace SlnUp.Tests;

using FluentAssertions;
using SlnUp.TestLibrary;
using SlnUp.Tests.Utilities;
using System.CommandLine;
using System.CommandLine.IO;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

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
        ProgramOptions? result = this.ConfigureAndInvoke(args);

        // Assert
        Assert.NotNull(result);
        result.Should().BeEquivalentTo(new ProgramOptions
        {
            Version = version
        });
    }

    [Fact]
    public void ConfigureNoParameters()
    {
        // Arrange
        string[] args = Array.Empty<string>();

        // Act
        ProgramOptions? result = this.ConfigureAndInvoke(args);

        // Assert
        Assert.Null(result);
        this.testConsole.Should().HaveOutputWritten();
        this.testConsole.GetErrorOutput().Should().StartWith("Required argument missing for command");
    }

    [Theory]
    [InlineData("2022", "--build-version", "17.0.31903.59")]
    [InlineData("--build-version", "17.0.31903.59", "2022")]
    public void ConfigureWithBuildVersion(params string[] args)
    {
        // Arrange
        const string expectedVersion = "2022";
        const string expectedBuildVersion = "17.0.31903.59";

        // Act
        ProgramOptions? result = this.ConfigureAndInvoke(args);

        // Assert
        Assert.NotNull(result);
        result.Should().BeEquivalentTo(new ProgramOptions
        {
            Version = expectedVersion,
            BuildVersion = Version.Parse(expectedBuildVersion),
        });
    }

    [Theory]
    [InlineData("--help")]
    [InlineData("-h")]
    [InlineData("-?")]
    public void ConfigureWithHelp(params string[] args)
    {
        // Act
        ProgramOptions? result = this.ConfigureAndInvoke(args);

        // Assert
        Assert.Null(result);
        this.testConsole.GetOutput().Should().StartWith("Description:");
        this.testConsole.Should().NotHaveErrorWritten();
    }

    [Theory]
    [InlineData("2022", "--path", "C:/solution.sln")]
    [InlineData("2022", "-p", "C:/solution.sln")]
    [InlineData("-p", "C:/solution.sln", "2022")]
    public void ConfigureWithPath(params string[] args)
    {
        // Arrange
        const string expectedVersion = "2022";
        string expectedFilePath = "C:/solution.sln".ToCrossPlatformPath();

        // Act
        ProgramOptions? result = this.ConfigureAndInvoke(args.ToCrossPlatformPath().ToArray());

        // Assert
        Assert.NotNull(result);
        result.Should().BeEquivalentTo(new ProgramOptions
        {
            Version = expectedVersion,
            SolutionPath = expectedFilePath,
        });
    }

    [Fact]
    public void ConfigureWithVersion()
    {
        // Arrange
        string[] args = new[] { "--version" };

        // Act
        ProgramOptions? result = this.ConfigureAndInvoke(args);

        // Assert
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

    /// <summary>
    /// Build version: > app 0.0 --build-version 0.0.0.0
    /// </summary>
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

    /// <summary>
    /// Build version with invalid version: > app 0 --build-version 0.0.0
    /// </summary>
    [Fact]
    public void TryGetSlnUpOptionsWithBuildVersionAndInvalidVersion()
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

    /// <summary>
    /// Minimal invocation: > app 0.0
    /// </summary>
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

    /// <summary>
    /// Multiple solution files available: > app 16.8
    /// </summary>
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

    /// <summary>
    /// app 16.8 --path C:\Missing.sln
    /// </summary>
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

    /// <summary>
    /// Multiple solution files available: > app 16.8
    /// </summary>
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

    /// <summary>
    /// app 16.8 --path .\MyProject.sln
    /// </summary>
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

    private ProgramOptions? ConfigureAndInvoke(string[] args)
        => this.ConfigureAndInvoke(args, out _);

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
