using CashControl.Api.Abstractions;
using CashControl.Api.Responses.Transactions;
using CashControl.Api.Responses.ValueObjects;
using CashControl.Domain.Accounts;
using CashControl.Domain.Primitives;
using CashControl.Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CashControl.Api.Feature.Accounts;

public class Details
{
    public record Response(
        Guid Id,
        string Name,
        MoneyResponse Balance,
        IEnumerable<TransactionsResponse> Transactions
    );

    public class GetAccountByIdEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app) =>
            app.MapGet("/{id:guid}", HandleAsync)
                .WithName("Accounts: Get by id")
                .WithSummary("Gets an account by its ID")
                .WithDescription("Retrieves the details of an account by its ID.");

        private static async Task<Results<NotFound<Result>, Ok<Result<Response>>>> HandleAsync(
            Guid id,
            CashControlDbContext context,
            CancellationToken cancellationToken
        )
        {
            AccountId accountId = AccountId.Create(id);

            Response? accountResponse = await context
                .Accounts.AsNoTracking()
                .Where(a => a.Id == accountId)
                .Select(a => new Response(
                    a.Id,
                    a.Name,
                    new MoneyResponse(a.Balance.Value, a.Balance.Currency.ToString()),
                    a.Transactions.Select(t => new TransactionsResponse(
                        t.Id,
                        t.Description,
                        new MoneyResponse(t.Amount.Value, t.Amount.Currency.ToString()),
                        t.Type,
                        t.Date
                    ))
                ))
                .SingleOrDefaultAsync(cancellationToken);

            if (accountResponse is null)
            {
                Result failureResult = Result.Failure(Errors.AccountNotFound(id));
                return TypedResults.NotFound(failureResult);
            }

            Result<Response> result = Result.Success(accountResponse);
            return TypedResults.Ok(result);
        }
    }
}
