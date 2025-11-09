using System.Text.RegularExpressions;
using CashControl.Api.Abstractions;
using CashControl.Domain.Errors;
using CashControl.Domain.Primitives;
using CashControl.Domain.Users;
using CashControl.Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PasswordEncryption = BCrypt.Net.BCrypt;

namespace CashControl.Api.Feature.Users;

public partial class Create
{
    public record Command(string Email, string Password);

    public record Response(Guid Id);

    public class CreateUserEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app) =>
            app.MapPost("", HandleAsync)
                .WithName("Users: Create")
                .WithSummary("Creates a new user")
                .WithDescription("Creates a new user with the given email and password.");

        private static async Task<
            Results<BadRequest<Result>, Created<Result<Response>>>
        > HandleAsync(
            Command command,
            CashControlDbContext context,
            CancellationToken cancellationToken
        )
        {
            Error? error = Validate(command);

            if (error is not null)
            {
                Result failureResult = Result.Failure(error);
                return TypedResults.BadRequest(failureResult);
            }

            Email email = Email.Create(command.Email);
            bool emailInUse = await context.Users.AnyAsync(
                u => u.Email == email,
                cancellationToken
            );
            if (emailInUse)
            {
                var failureResult = Result.Failure(UserErrors.EmailAlreadyExists);
                return TypedResults.BadRequest(failureResult);
            }

            string passwordHash = PasswordEncryption.HashPassword(command.Password);

            User user = User.Create(email, passwordHash);

            context.Users.Add(user);
            await context.SaveChangesAsync(cancellationToken);

            var result = Result.Success(new Response(user.Id));
            return TypedResults.Created($"/users/{user.Id.Value}", result);
        }

        private static Error? Validate(Command command)
        {
            if (string.IsNullOrWhiteSpace(command.Email))
                return Error.ValidationError("The field Email must be informed.");

            if (string.IsNullOrWhiteSpace(command.Password))
                return Error.ValidationError("The field Password must be informed.");

            if (!PasswordValidationRegex().IsMatch(command.Password))
                return Error.ValidationError(
                    "Password must be strong (8 to 64 chars, upper, lower, number, special)."
                );

            return null;
        }
    }

    [GeneratedRegex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9])[\x20-\x7E]{8,64}$")]
    private static partial Regex PasswordValidationRegex();
}
