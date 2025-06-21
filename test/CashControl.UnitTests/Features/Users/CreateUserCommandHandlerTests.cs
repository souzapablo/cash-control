using CashControl.API.Features.Users.Commands.Create;

namespace CashControl.UnitTests.Features.Users;

public class CreateUserCommandHandlerTests
{
    private static readonly IUserRepository UserRepository = Substitute.For<IUserRepository>();
    private static readonly IUnitOfWork UnitOfWork = Substitute.For<IUnitOfWork>();
    
    [Fact(DisplayName = "Create new user successfully")]
    public async Task ShouldCreateUser_WhenRequestIsValid()
    {
        // Arrange
        var command = new CreateUserCommand("test@email.com", "test", "test", "test");
        var commandHandler = new CreateUserCommandHandler(UserRepository, UnitOfWork);

        // Act
        await commandHandler.Handle(command, CancellationToken.None);
        
        // Assert
        UserRepository.Received().Save(Arg.Any<User>());
        await UnitOfWork.Received().CommitAsync(Arg.Any<CancellationToken>());
    }
}