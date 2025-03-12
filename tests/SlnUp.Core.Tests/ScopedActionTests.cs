namespace SlnUp.Core.Tests;

public class ScopedActionTests
{
    [Test]
    public async Task ScopedAction_Invoked()
    {
        // Arrange
        int actionCalled = 0;
        ScopedAction action = new(() => ++actionCalled);

        // Act
        action.Dispose();
        action.Dispose();

        // Assert
        await Assert.That(actionCalled).IsEqualTo(1);
    }

    [Test]
    public async Task ScopedAction_InvokedFromUsing()
    {
        // Arrange
        bool actionCalled = false;

        // Act
        {
            using ScopedAction action = new(() => actionCalled = true);
        }

        // Assert
        await Assert.That(actionCalled).IsTrue();
    }

    [Test]
    public void ScopedAction_Null()
    {
        // Arrange
        ScopedAction action = new(null!);

        // Act and assert
        action.Dispose();
    }
}
