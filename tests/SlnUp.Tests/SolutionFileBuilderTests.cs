namespace SlnUp.Tests;

public class SolutionFileBuilderTests
{
    [Fact]
    public void Build()
    {
        // Arrange
        SolutionFileBuilder fileBuilder = new();
        const string expectedContent = @"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 16
VisualStudioVersion = 16.0.28701.123
MinimumVisualStudioVersion = 10.0.40219.1
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
EndGlobal
";

        // Act
        string actualContent = fileBuilder.Build();

        // Assert
        actualContent.Should().Be(expectedContent);
    }

    [Fact]
    public void Build_ForVS15()
    {
        // Arrange
        SolutionFileBuilder fileBuilder = new(Version.Parse("15.0.28701.123"));
        const string expectedContent = @"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio 15
VisualStudioVersion = 15.0.28701.123
MinimumVisualStudioVersion = 10.0.40219.1
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
EndGlobal
";

        // Act
        string actualContent = fileBuilder.Build();

        // Assert
        actualContent.Should().Be(expectedContent);
    }

    [Fact]
    public void Build_WithMinimumHeader()
    {
        // Arrange
        SolutionFileBuilder fileBuilder = new SolutionFileBuilder()
            .ConfigureMinimumHeader();
        const string expectedContent = @"
Microsoft Visual Studio Solution File, Format Version 12.00
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
EndGlobal
";

        // Act
        string actualContent = fileBuilder.Build();

        // Assert
        actualContent.Should().Be(expectedContent);
    }

    [Fact]
    public void Build_WithoutBody()
    {
        // Arrange
        SolutionFileBuilder fileBuilder = new SolutionFileBuilder()
            .ExcludeBody();
        const string expectedContent = @"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 16
VisualStudioVersion = 16.0.28701.123
MinimumVisualStudioVersion = 10.0.40219.1
";

        // Act
        string actualContent = fileBuilder.Build();

        // Assert
        actualContent.Should().Be(expectedContent);
    }

    [Fact]
    public void Build_WithoutFileFormatVersion()
    {
        // Arrange
        SolutionFileBuilder fileBuilder = new SolutionFileBuilder()
            .ExcludeFileFormatVersion();
        const string expectedContent = @"
# Visual Studio Version 16
VisualStudioVersion = 16.0.28701.123
MinimumVisualStudioVersion = 10.0.40219.1
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
EndGlobal
";

        // Act
        string actualContent = fileBuilder.Build();

        // Assert
        actualContent.Should().Be(expectedContent);
    }

    [Fact]
    public void Build_WithoutSolutionIconVersion()
    {
        // Arrange
        SolutionFileBuilder fileBuilder = new SolutionFileBuilder()
            .ExcludeSolutionIconVersion();
        const string expectedContent = @"
Microsoft Visual Studio Solution File, Format Version 12.00
VisualStudioVersion = 16.0.28701.123
MinimumVisualStudioVersion = 10.0.40219.1
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
EndGlobal
";

        // Act
        string actualContent = fileBuilder.Build();

        // Assert
        actualContent.Should().Be(expectedContent);
    }

    [Fact]
    public void Build_WithoutVisualStudioFullVersion()
    {
        // Arrange
        SolutionFileBuilder fileBuilder = new SolutionFileBuilder()
            .ExcludeVisualStudioFullVersion();
        const string expectedContent = @"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 16
MinimumVisualStudioVersion = 10.0.40219.1
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
EndGlobal
";

        // Act
        string actualContent = fileBuilder.Build();

        // Assert
        actualContent.Should().Be(expectedContent);
    }

    [Fact]
    public void Build_WithoutVisualStudioMinimumVersion()
    {
        // Arrange
        SolutionFileBuilder fileBuilder = new SolutionFileBuilder()
            .ExcludeVisualStudioMinimumVersion();
        const string expectedContent = @"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 16
VisualStudioVersion = 16.0.28701.123
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
EndGlobal
";

        // Act
        string actualContent = fileBuilder.Build();

        // Assert
        actualContent.Should().Be(expectedContent);
    }
}
