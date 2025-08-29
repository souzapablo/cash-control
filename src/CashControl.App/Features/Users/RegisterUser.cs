using CashControl.App.Abstractions;
using CashControl.App.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using BCryptNet = BCrypt.Net.BCrypt;

namespace CashControl.App.Features.Users;

public record RegisterUserCommand(string Username, string Email, string Password);
public record RegisterUserResponse(Guid Id);

public class RegisterUserHandler(IConfiguration configuration,
    AppDbContext repository)
{
    public async Task<Result<RegisterUserResponse>> HandleAsync(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var emailRegistered = await repository.Users
            .AnyAsync(u => u.Email == command.Email, cancellationToken);

        if (emailRegistered)
            return Result.Failure<RegisterUserResponse>(UserErrors.EmailAlreadyRegistered);

        var pepper = configuration["Security:PasswordPepper"];
        if (string.IsNullOrWhiteSpace(pepper))
            throw new InvalidOperationException("Password pepper configuration is missing");

        var passwordHash = BCryptNet.HashPassword(command.Password + pepper);

        var user = User.Register(command.Username, passwordHash, command.Email);

        repository.Add(user);
        await repository.SaveChangesAsync(cancellationToken);

        var response = new RegisterUserResponse(user.Id.Value);

        return Result.Success(response);
    }
}

public class RegisterUserEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapPost("", HandleAsync)
            .WithName("User: Create")
            .WithSummary("Create a new user")
            .WithDescription("Create a new user")
            .WithOrder(1)
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);

    private static async Task<IResult> HandleAsync(RegisterUserHandler handler, RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(request, cancellationToken);

        if (!result.IsSuccess)
            return TypedResults.BadRequest(result);

        return TypedResults.Created($"api/v1/users/{result.Value?.Id}", result);
    }

}