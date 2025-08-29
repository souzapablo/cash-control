using CashControl.UnitTests.Builders;

namespace CashControl.UnitTests.Entities;

public class EntityTests
{
    [Fact(DisplayName = "Deactivate entity should set IsActive to false and update LastUpdate")]
    public void Should_SetIsActiveToFalse_And_UpdateLastUpdate_When_Deactivated()
    {
        // Arrange
        var user = new UserBuilder().Build();

        // Act
        user.Deactivate();

        // Assert
        Assert.False(user.IsActive);
        Assert.NotNull(user.LastUpdate);
    }

    [Fact(DisplayName = "Activate entity should set IsActive to true and update LastUpdate")]
    public void Should_SetIsActiveToTrue_And_UpdateLastUpdate_When_Activated()
    {
        // Arrange
        var user = new UserBuilder().Build();

        // Act
        user.Activate();

        // Assert
        Assert.True(user.IsActive);
        Assert.NotNull(user.LastUpdate);
    }
}