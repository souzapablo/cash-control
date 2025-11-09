using CashControl.Api.Abstractions;
using CashControl.Domain.Errors;
using CashControl.Domain.Primitives;
using CashControl.Domain.Users;
using CashControl.Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PasswordEncryption = BCrypt.Net.BCrypt;

namespace CashControl.Api.Feature.Auth;

public class Login
{
    public record Command(string Email, string Password);

    public record Response(string AccessToken);

    public class LoginEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app) =>
            app.MapPost("login", HandleAsync)
                .WithName("Authentication: Login")
                .WithSummary("Logs in a user")
                .WithDescription("Authenticates a user with the given email and password.");

        private static async Task<Results<BadRequest<Result>, Ok<Result<Response>>>> HandleAsync(
            Command command,
            CashControlDbContext context,
            CancellationToken cancellationToken
        )
        {
            Email email = Email.Create(command.Email);
            User? user = await context.Users.FirstOrDefaultAsync(
                user => user.Email == email,
                cancellationToken
            );

            if (user is null)
            {
                var result = Result.Failure(UserErrors.InvalidCredentials);
                return TypedResults.BadRequest(result);
            }

            bool passwordValid = PasswordEncryption.Verify(command.Password, user?.PasswordHash);

            if (!passwordValid)
            {
                var result = Result.Failure(UserErrors.InvalidCredentials);
                return TypedResults.BadRequest(result);
            }

            var fakeToken = Guid.NewGuid().ToString();
            return TypedResults.Ok(Result.Success(new Response(fakeToken)));
        }
    }
}
