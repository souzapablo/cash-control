using CashControl.App.Abstractions;
using BCryptNet = BCrypt.Net.BCrypt;

namespace CashControl.App.Features.Users.Commands;

public record RegisterUserCommand(string Username, string Email, string Password);
public record RegisterUserResponse(Guid Id);
public interface IRegisterUserHandler
{
    Task<Result<RegisterUserResponse>> HandleAsync(RegisterUserCommand command, CancellationToken cancellationToken = default);
}
public class RegisterUserHandler(IConfiguration configuration,
    IUserRepository repository) : IRegisterUserHandler
{
    public async Task<Result<RegisterUserResponse>> HandleAsync(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var emailRegistered = await repository.ExistWithEmailAsync(command.Email, cancellationToken);
        if (emailRegistered)
            return Result.Failure<RegisterUserResponse>(UserErrors.EmailAlreadyRegistered);

        var pepper = configuration["Security:PasswordPepper"];
        if (string.IsNullOrWhiteSpace(pepper))
            throw new UserException(UserErrors.PepperNotFound);

        var passwordHash = BCryptNet.HashPassword(command.Password + pepper);

        var user = User.Register(command.Username, passwordHash, command.Email);

        repository.Register(user);
        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success(user.ToResponse());
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

    private static async Task<IResult> HandleAsync(IRegisterUserHandler handler, RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(request, cancellationToken);

        if (!result.IsSuccess)
            return TypedResults.BadRequest();
        
        return TypedResults.Created($"api/v1/users/{result.Value?.Id}", result);
    }
    
}

public static class Converter 
{
    public static RegisterUserResponse ToResponse(this User user) => new(user.Id.Value);
}