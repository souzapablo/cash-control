using CashControl.API.Abstractions.Messaging;
using CashControl.API.Features.Users.Commands.Create;
using Microsoft.AspNetCore.Mvc;

namespace CashControl.API.Controllers;

[Route("api/v1/users")]
public class UsersController(ICommandHandler<CreateUserCommand, long> createUserCommand) : ControllerBase
{
    
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateUserCommand command, CancellationToken cancellationToken)
    {
        var id = await createUserCommand.Handle(command, cancellationToken);
        return CreatedAtAction("GetUser", new { id }, id);
    }
}