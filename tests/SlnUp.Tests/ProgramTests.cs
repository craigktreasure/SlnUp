namespace SlnUp.Tests;

using SlnUp;
using SlnUp.TestLibrary;

public class ProgramTests
{
    private const int FailedExitCode = 1;

    private const int NormalExitCode = 0;

    [Test]
    public async Task Main_NoLocalFile()
    {
        // Arrange
        using ScopedDirectory directory = TemporaryDirectory.CreateRandom();
        string[] args = ["2022"];
        using IDisposable _ = directory.SetAsScopedWorkingDirectory();

        // Act
        int exitCode = Program.Main(args);

        // Assert
        await Assert.That(exitCode).IsEqualTo(FailedExitCode);
    }

    [Test]
    public async Task Main_WithFilePath()
    {
        // Arrange
        using ScopedFile file = TemporaryFile.CreateRandomWithExtension("sln");
        SolutionFileBuilder solutionFileBuilder = new(Version.Parse("16.0.30114.105"));
        SolutionFile solutionFile = solutionFileBuilder.BuildToSolutionFile(file.FileSystem, file.Path);
        string[] args =
        [
            "2022",
            "--path",
            file.Path,
        ];

        // Act
        int exitCode = Program.Main(args);

        // Assert
        await Assert.That(exitCode).IsEqualTo(NormalExitCode);
        solutionFile.Reload();
        await Assert.That(solutionFile.FileHeader.LastVisualStudioMajorVersion).IsEqualTo(17);
    }

    [Test]
    public async Task Main_WithInvalidFile()
    {
        // Arrange
        using ScopedFile file = TemporaryFile.CreateRandomWithExtension("sln");
        string[] args =
        [
            "2022",
            "--path",
            file.Path,
        ];

        // Act
        int exitCode = Program.Main(args);

        // Assert
        await Assert.That(exitCode).IsEqualTo(FailedExitCode);
    }

    [Test]
    public async Task Main_WithLocalFile()
    {
        // Arrange
        using ScopedDirectory directory = TemporaryDirectory.CreateRandom();
        string solutionFilePath = directory.GetRandomFilePathWithExtension("sln");
        SolutionFileBuilder solutionFileBuilder = new(Version.Parse("16.0.30114.105"));
        SolutionFile solutionFile = solutionFileBuilder.BuildToSolutionFile(directory.FileSystem, solutionFilePath);
        string[] args = ["2022"];
        using IDisposable _ = directory.SetAsScopedWorkingDirectory();

        // Act
        int exitCode = Program.Main(args);

        // Assert
        await Assert.That(exitCode).IsEqualTo(NormalExitCode);
        solutionFile.Reload();
        await Assert.That(solutionFile.FileHeader.LastVisualStudioMajorVersion).IsEqualTo(17);
    }
}
