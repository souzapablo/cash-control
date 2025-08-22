using CashControl.App.Features.Users;
using CashControl.App.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace CashControl.UnitTests.Features.Users;

public class RegisterUserTests
{
    private readonly IConfiguration _configuration = Substitute.For<IConfiguration>();

    [Fact(DisplayName = "Handle should register a new user")]
    public async Task Should_RegisterANewUser_When_RequestIsValid()
    {
        // Arrange
        var (handler, context) = CreateHandler(withPepper: true);

        // Act
        var result = await handler.HandleAsync(Command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Value?.Id);
        Assert.Null(result.Error);

        context.Dispose();
    }

    [Fact(DisplayName = "Handle should not register a user with existing email")]
    public async Task Should_NotRegisterAUser_When_EmailIsAlreadyRegistered()
    {
        // Arrange
        var (handler, context) = CreateHandler(withPepper: true);
        var existingUser = User.Register("existing", "hashedpassword", "test@email.com");
        context.Users.Add(existingUser);
        await context.SaveChangesAsync();

        // Act
        var result = await handler.HandleAsync(Command, CancellationToken.None);

        // Assert
        Assert.Equal(UserErrors.EmailAlreadyRegistered, result.Error);
        Assert.False(result.IsSuccess);

        context.Dispose();
    }

    [Fact(DisplayName = "Handle should not register an user without pepper")]
    public async Task Should_NotRegisterAnUser_When_PepperIsNotFound()
    {
        // Arrange
        var (handler, context) = CreateHandler(withPepper: false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.HandleAsync(Command, CancellationToken.None));
        Assert.Equal("Password pepper configuration is missing", exception.Message);
        
        context.Dispose();
    }

    private static RegisterUserCommand Command => new("Test", "test@email.com", "password");

    private static (RegisterUserHandler handler, AppDbContext context) CreateHandler(bool withPepper = true)
    {
        var configuration = Substitute.For<IConfiguration>();
        if (withPepper)
            configuration["Security:PasswordPepper"].Returns("test-pepper-value");

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        var handler = new RegisterUserHandler(configuration, context);

        return (handler, context);
    }
}
