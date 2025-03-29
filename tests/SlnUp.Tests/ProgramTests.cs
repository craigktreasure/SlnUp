namespace SlnUp.Tests;

using SlnUp;
using SlnUp.TestLibrary;

/// <remarks>
/// This class contains tests that change the working directory, which causes problems when running in parallel in TUnit.
/// For this reason, we set ParallelGroup() to nameof(ProgramTests) to prevent parallel execution with other test classes
/// and set NotInParallel to each test to prevent parallel execution within the class.
/// </remarks>
[ParallelGroup(nameof(ProgramTests))]
public class ProgramTests
{
    private const int FailedExitCode = 1;

    private const int NormalExitCode = 0;

    [Test, NotInParallel]
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

    [Test, NotInParallel]
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

    [Test, NotInParallel]
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

    [Test, NotInParallel]
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
