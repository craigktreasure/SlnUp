﻿namespace SlnUp.Core.Tests;

public class ScopedActionTests
{
    [Fact]
    public void ScopedAction_Invoked()
    {
        // Arrange
        int actionCalled = 0;
        ScopedAction action = new(() => ++actionCalled);

        // Act
        action.Dispose();
        action.Dispose();

        // Assert
        Assert.Equal(1, actionCalled);
    }

    [Fact]
    public void ScopedAction_InvokedFromUsing()
    {
        // Arrange
        bool actionCalled = false;

        // Act
        {
            using ScopedAction action = new(() => actionCalled = true);
        }

        // Assert
        Assert.True(actionCalled);
    }

    [Fact]
    public void ScopedAction_Null()
    {
        // Arrange
        ScopedAction action = new(null!);

        // Act and assert
        action.Dispose();
    }
}
