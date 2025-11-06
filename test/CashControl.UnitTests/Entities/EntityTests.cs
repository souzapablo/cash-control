using CashControl.Domain.Accounts;
using Xunit;

namespace CashControl.UnitTests.Entities;

public class EntityTests
{
    [Fact(DisplayName = "Delete should mark the entity as inactive")]
    public void Delete_ShouldMarkEntityAsInactive()
    {
        // Arrange
        var entity = Account.Create("Test Account");

        // Act
        entity.Delete();

        // Assert
        Assert.False(entity.IsActive);
        Assert.NotNull(entity.LastUpdate);
    }
}
