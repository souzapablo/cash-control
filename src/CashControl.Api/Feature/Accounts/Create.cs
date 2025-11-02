using CashControl.Api.Abstractions;
using CashControl.Domain.Accounts;
using CashControl.Domain.Primitives;
using CashControl.Infrastructure.Data;

namespace CashControl.Api.Feature.Accounts;

public class Create
{
    public record Command(string Name);
    public record Response(Guid Id);

    public class CreateAccountEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
            => app.MapPost("/", HandleAsync)
                .WithName("Accounts: Create")
                .WithSummary("Creates a new account")
                .WithDescription("Creates a new account with the given name for the user with zero balance.")
                .WithOrder(2)
                .Produces<Result<Response>>();

        private static async Task<IResult> HandleAsync(
            Command command,
            CashControlDbContext context,
            CancellationToken cancellationToken)
        {
            Error? error = Validate(command);

            if (error is not null)
            {
                var failureResult = Result.Failure(error);
                return Results.BadRequest(failureResult);
            }

            Account account = Account.Create(command.Name);

            context.Accounts.Add(account);
            await context.SaveChangesAsync(cancellationToken);

            Result<Response> result = Result.Success(new Response(account.Id.Value));

            return Results.Created($"accounts/{result.Value.Id}", result);
        }

        private static Error? Validate(Command command)
        {
            if (string.IsNullOrWhiteSpace(command.Name))
                return Error.ValidationError("The field Name must be informed.");

            if (command.Name.Length > 200)
                return Error.ValidationError("The field Name must be a string with a maximum length of '200'.");

            return null;
        }
    }
    
}