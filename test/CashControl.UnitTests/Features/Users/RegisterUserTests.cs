using CashControl.App.Features.Users;
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
        SetSecurityReturn();
        _repository.ExistWithEmailAsync("test@email.com", CancellationToken.None)
            .Returns(false);

        // Act
        var result = await Handler.HandleAsync(Command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Value?.Id);
        Assert.Null(result.Error);
    }

    [Fact(DisplayName = "Handle should not register a user with existing email")]
    public async Task Should_NotRegisterAUser_When_EmailIsAlreadyRegistered()
    {
        // Arrange
        SetSecurityReturn();
        _repository.ExistWithEmailAsync("test@email.com", CancellationToken.None)
            .Returns(true);

        // Act
        var result = await Handler.HandleAsync(Command, CancellationToken.None);

        // Assert
        Assert.Equal(UserErrors.EmailAlreadyRegistered, result.Error);
        Assert.False(result.IsSuccess);
        _repository.DidNotReceive().Register(Arg.Any<User>());
    }

    [Fact(DisplayName = "Handle should not register an user without pepper")]
    public async Task Should_NotRegisterAnUser_When_PepperIsNotFound()
    {
        // Arrange
        _repository.ExistWithEmailAsync("test@email.com", CancellationToken.None)
            .Returns(false);

        // Act
        var action = async () => await Handler.HandleAsync(Command, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<UserException>(action);
    }

    private static RegisterUserCommand Command => new("Test", "test@email.com", "password");
    private RegisterUserHandler Handler => new(_configuration, _repository);
    private void SetSecurityReturn() =>
        _configuration["Security:PasswordPepper"].Returns("test-pepper-value");
}
