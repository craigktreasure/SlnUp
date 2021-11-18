namespace SlnUp.Core.Tests;

using FluentAssertions;
using Xunit;

public class ArgumentTests
{
    [Fact]
    public void NotNull()
    {
        // Arrange
        int[] objectValue = Array.Empty<int>();

        // Act and assert
        Argument.NotNull(objectValue);
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("foo")]
    public void NotNullOrEmpty(string objectValue)
    {
        // Act and assert
        Argument.NotNullOrEmpty(objectValue);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void NotNullOrEmptyThrows(string? objectValue)
    {
        // Act
        ArgumentException ex = Assert.Throws<ArgumentException>(() => Argument.NotNullOrEmpty(objectValue));

        // Assert
        ex.ParamName.Should().Be(nameof(objectValue));
    }

    [Fact]
    public void NotNullOrWhiteSpace()
    {
        // Act and assert
        Argument.NotNullOrWhiteSpace("foo");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void NotNullOrWhiteSpaceThrows(string? objectValue)
    {
        // Act
        ArgumentException ex = Assert.Throws<ArgumentException>(() => Argument.NotNullOrWhiteSpace(objectValue));

        // Assert
        ex.ParamName.Should().Be(nameof(objectValue));
    }

    [Fact]
    public void NotNullThrows()
    {
        // Arrange
        object? objectValue = null;

        // Act
        ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => Argument.NotNull(objectValue));

        // Assert
        ex.ParamName.Should().Be(nameof(objectValue));
    }
}
