using CashControl.Api.Abstractions;
using CashControl.Api.Responses.ValueObjects;
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
        public static void Map(IEndpointRouteBuilder app) =>
            app.MapGet("/", HandleAsync)
                .WithName("Accounts: List")
                .WithSummary("List accounts")
                .WithDescription("Retrieves a list of all active accounts.");

        private static async Task<Ok<Result<List<Response>>>> HandleAsync(
            CashControlDbContext context,
            CancellationToken cancellationToken
        )
        {
            List<Response> accountsResponse = await context
                .Accounts.AsNoTracking()
                .Select(a => new Response(
                    a.Id.Value,
                    a.Name,
                    new MoneyResponse(a.Balance.Value, a.Balance.Currency.ToString())
                ))
                .ToListAsync(cancellationToken);

            return TypedResults.Ok(Result.Success(accountsResponse));
        }
    }
}
