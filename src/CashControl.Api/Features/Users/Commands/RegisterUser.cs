using CashControl.Api.Abstractions;

namespace CashControl.Api.Features.Users.Commands;

public record Request(string Username, string Email, string Password);
public record Response(Guid Id);
public class RegisterUserCommand
{
    public async Task<Response> HandleAsync(Request request, CancellationToken cancellationToken = default)
    {
        var user = User.Register(request.Username, request.Password, request.Email);

        return user.ToResponse();
    }
}

public class RegisterUserEnpdoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapPost("", HandleAsync)
            .WithName("User: Create")
            .WithSummary("Create a new user")
            .WithDescription("Create a new user")
            .WithOrder(1)
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);

    private static async Task<IResult> HandleAsync(Request request, CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand();
        var result = command.HandleAsync(request, cancellationToken);
        return TypedResults.Created($"api/v1/users/{result.Id}", result);
    }
}

public static class Converter 
{
    public static Response ToResponse(this User user) => new(user.Id.Value);
}