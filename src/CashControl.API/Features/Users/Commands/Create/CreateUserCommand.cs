using CashControl.API.Abstractions.Messaging;

namespace CashControl.API.Features.Users.Commands.Create;

public record CreateUserCommand(string Email, string Username, string FirstName, string LastName) : ICommand<long>;