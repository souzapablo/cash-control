using CashControl.App.Features.Users.Commands;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace CashControl.UnitTests.Features.Users;

public class RegisterUserTests
{
    [Fact(DisplayName = "Handle should register a new user")]
    public async Task Should_RegisterANewUser_When_RequestIsValid()
    {
        // Arrange
        var configuration = Substitute.For<IConfiguration>();
        configuration["Security:PasswordPepper"].Returns("test-pepper-value");
        var request = new RegisterUserCommand("Test", "test@email.com", "password");
        var handler = new RegisterUserHandler(configuration);

        // Act
        var result = await handler.HandleAsync(request, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Value?.Id);
        Assert.Null(result.Error);
    }
}
