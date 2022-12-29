namespace SlnUp.Tests;

using SlnUp;
using SlnUp.TestLibrary;

public class ProgramTests
{
    private const int FailedExitCode = 1;

    private const int NormalExitCode = 0;

    [Fact]
    public void Main_NoLocalFile()
    {
        // Arrange
        using ScopedDirectory directory = TemporaryDirectory.CreateRandom();
        string[] args = new[] { "2022" };
        using IDisposable _ = directory.SetAsScopedWorkingDirectory();

        // Act
        int exitCode = Program.Main(args);

        // Assert
        Assert.Equal(FailedExitCode, exitCode);
    }

    [Fact]
    public void Main_WithFilePath()
    {
        // Arrange
        using ScopedFile file = TemporaryFile.CreateRandomWithExtension("sln");
        SolutionFileBuilder solutionFileBuilder = new(Version.Parse("16.0.30114.105"));
        SolutionFile solutionFile = solutionFileBuilder.BuildToSolutionFile(file.FileSystem, file.Path);
        string[] args = new[]
        {
            "2022",
            "--path", file.Path,
        };

        // Act
        int exitCode = Program.Main(args);

        // Assert
        Assert.Equal(NormalExitCode, exitCode);
        solutionFile.Reload();
        solutionFile.FileHeader.LastVisualStudioMajorVersion.Should().Be(17);
    }

    [Fact]
    public void Main_WithInvalidFile()
    {
        // Arrange
        using ScopedFile file = TemporaryFile.CreateRandomWithExtension("sln");
        string[] args = new[]
        {
            "2022",
            "--path", file.Path,
        };

        // Act
        int exitCode = Program.Main(args);

        // Assert
        Assert.Equal(FailedExitCode, exitCode);
    }

    [Fact]
    public void Main_WithLocalFile()
    {
        // Arrange
        using ScopedDirectory directory = TemporaryDirectory.CreateRandom();
        string solutionFilePath = directory.GetRandomFilePathWithExtension("sln");
        SolutionFileBuilder solutionFileBuilder = new(Version.Parse("16.0.30114.105"));
        SolutionFile solutionFile = solutionFileBuilder.BuildToSolutionFile(directory.FileSystem, solutionFilePath);
        string[] args = new[] { "2022" };
        using IDisposable _ = directory.SetAsScopedWorkingDirectory();

        // Act
        int exitCode = Program.Main(args);

        // Assert
        Assert.Equal(NormalExitCode, exitCode);
        solutionFile.Reload();
        solutionFile.FileHeader.LastVisualStudioMajorVersion.Should().Be(17);
    }
}
