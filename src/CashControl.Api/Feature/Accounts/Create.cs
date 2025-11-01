using CashControl.Api.Abstractions;
using CashControl.Domain.Accounts;
using CashControl.Domain.ValueObjects;
using CashControl.Infrastructure.Data;

namespace CashControl.Api.Feature.Accounts;

public class Create
{
    public record Command(string Name);
    public record Result(Guid Id);

    public class CreateAccountEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
            => app.MapPost("/", HandleAsync)
                .WithName("Accounts: Create")
                .WithSummary("Creates a new account")
                .WithDescription("Creates a new account with the given name for the user with zero balance.")
                .WithOrder(1)
                .Produces<Result>();
            
        private static async Task<IResult> HandleAsync(
            Command command,
            CashControlDbContext context,
            CancellationToken cancellationToken)
        {
            Account account = Account.Create(command.Name);

            context.Accounts.Add(account);
            await context.SaveChangesAsync(cancellationToken);

            Result result = new(account.Id);

            return Results.Created($"accounts/{result.Id}", result);
        }
    }
}