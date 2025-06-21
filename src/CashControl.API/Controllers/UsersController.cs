using CashControl.API.Abstractions.Messaging;
using CashControl.API.Features.Users.Commands.Create;
using CashControl.API.Models.Users;
using Microsoft.AspNetCore.Mvc;

namespace CashControl.API.Controllers;

[Route("api/v1/users")]
[ApiController]
public class UsersController(ICommandHandler<CreateUserCommand, long> createUserCommand) : ControllerBase
{
    
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateUserInputModel input, CancellationToken cancellationToken)
    {
        var id = await createUserCommand.Handle(input.ToCommand(), cancellationToken);
        return CreatedAtAction("GetUser", new { id }, id);
    }
}