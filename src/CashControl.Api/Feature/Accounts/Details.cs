using CashControl.Api.Abstractions;
using CashControl.Api.Responses.ValueObjects;
using CashControl.Domain.Accounts;
using CashControl.Domain.Primitives;
using CashControl.Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CashControl.Api.Feature.Accounts;

public class Details
{
    public record Response(Guid Id, string Name, MoneyResponse Balance);

    public class GetAccountByIdEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
            => app.MapGet("/{id:guid}", HandleAsync)
                .WithName("Accounts: Get by id")
                .WithSummary("Gets an account by its ID")
                .WithDescription("Retrieves the details of an account by its ID.");

        private static async Task<Results<NotFound<Result>, Ok<Result<Response>>>> HandleAsync(
            Guid id,
            CashControlDbContext context,
            CancellationToken cancellationToken)
        {
            AccountId accountId = AccountId.Create(id);

            Account? account = await context.Accounts
                .AsNoTracking()
                .SingleOrDefaultAsync(a => a.Id == accountId, cancellationToken);

            if (account is null)
            {
                Result failureResult = Result.Failure(Errors.AccountNotFound(id));
                return TypedResults.NotFound(failureResult);
            }

            Response response = new(
                account.Id.Value,
                account.Name,
                new MoneyResponse(account.Balance));
                
            Result<Response> result = Result.Success(response);
            return TypedResults.Ok(result);
        }
    }
}