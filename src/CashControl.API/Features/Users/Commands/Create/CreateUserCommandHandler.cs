using CashControl.API.Abstractions;
using CashControl.API.Abstractions.Messaging;
using CashControl.API.Domain.Entities;
using CashControl.API.Domain.Repositories;

namespace CashControl.API.Features.Users.Commands.Create;

public class CreateUserCommandHandler(IUserRepository repository, IUnitOfWork unitOfWork) : ICommandHandler<CreateUserCommand, long>
{
    public async Task<long> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var user = new User(command.Email, command.Username,command.FirstName, command.LastName);
        repository.Save(user);
        await unitOfWork.CommitAsync(cancellationToken);
        return user.Id;
    }
}