namespace SlnUp.Core.Tests;

using FluentAssertions;
using Xunit;

public class ScopedActionTests
{
    [Fact]
    public void ScopedActionInvoked()
    {
        // Arrange
        int actionCalled = 0;
        ScopedAction action = new(() => ++actionCalled);

        // Act
        action.Dispose();
        action.Dispose();

        // Assert
        actionCalled.Should().Be(1);
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
