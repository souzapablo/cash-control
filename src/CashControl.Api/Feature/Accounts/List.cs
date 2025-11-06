using CashControl.Api.Abstractions;
using CashControl.Api.Responses.ValueObjects;
using CashControl.Domain.Accounts;
using CashControl.Domain.Primitives;
using CashControl.Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CashControl.Api.Feature.Accounts;

public class List
{
    public record Response(Guid Id, string Name, MoneyResponse Balance);

    public class ListAccountsEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
            => app.MapGet("/", HandleAsync)
                .WithName("Accounts: List")
                .WithSummary("List accounts")   
                .WithDescription("Retrieves a list of all active accounts.");

        private static async Task<Ok<Result<IEnumerable<Response>>>> HandleAsync(
            CashControlDbContext context,
            CancellationToken cancellationToken
        )
        {
            List<Account> accounts = await context
                .Accounts
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            IEnumerable<Response> responseItems = accounts
                .Select(account => new Response(
                    account.Id.Value,
                    account.Name,
                    new MoneyResponse(account.Balance)));

            return TypedResults.Ok(Result.Success(responseItems));
        }
    }
}
