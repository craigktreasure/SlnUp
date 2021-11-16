namespace SlnUp.Core.Tests;

using FluentAssertions;
using Xunit;

public class ScopedActionTests
{
    [Fact]
    public void ScopedActionInvoked()
    {
        // Arrange
        bool actionCalled = false;
        ScopedAction action = new(() => actionCalled = true);

        // Act
        action.Dispose();

        // Assert
        actionCalled.Should().BeTrue();
    }

    [Fact]
    public void ScopedActionInvokedFromUsing()
    {
        // Arrange
        bool actionCalled = false;

        // Act
        {
            using ScopedAction action = new(() => actionCalled = true);
        }

        // Assert
        actionCalled.Should().BeTrue();
    }
}
