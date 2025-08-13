using CashControl.App.Features.Users;
using CashControl.App.Features.Users.Commands;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace CashControl.UnitTests.Features.Users;

public class RegisterUserTests
{
    private readonly IConfiguration _configuration = Substitute.For<IConfiguration>();
    private readonly IUserRepository _repository = Substitute.For<IUserRepository>();

    [Fact(DisplayName = "Handle should register a new user")]
    public async Task Should_RegisterANewUser_When_RequestIsValid()
    {
        // Arrange
        _configuration["Security:PasswordPepper"].Returns("test-pepper-value");
        
        var request = new RegisterUserCommand("Test", "test@email.com", "password");
        var handler = new RegisterUserHandler(_configuration, _repository);

        // Act
        var result = await handler.HandleAsync(request, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Value?.Id);
        Assert.Null(result.Error);
    }
}
