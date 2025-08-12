using CashControl.Api.Features.Users.Commands;

namespace CashControl.UnitTests.Features.Users;

public class RegisterUserTests
{
    [Fact(DisplayName = "Handle should register a new user")]
    public async Task Should_RegisterANewUser_When_RequestIsValid()
    {
        // Arrange
        var request = new Request("Test", "test@email.com", "password");
        var command = new RegisterUserCommand();

        // Act
        var result = await command.HandleAsync(request, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
    }
}
