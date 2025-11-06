using CashControl.Api.Abstractions;
using CashControl.Domain.Accounts;
using CashControl.Domain.Primitives;
using CashControl.Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CashControl.Api.Feature.Accounts;

public class Delete
{
    public record Command(Guid Id);

    public class DeleteAccountEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
            => app.MapDelete("/{id:guid}", HandleAsync)
                .WithName("Accounts: Delete")
                .WithSummary("Deletes an account")
                .WithDescription("Deletes an account by its ID.")
                .WithOrder(3)
                .Produces(StatusCodes.Status204NoContent)
                .Produces<Result>(StatusCodes.Status404NotFound);
    
        private static async Task<Results<NoContent, NotFound<Result>>> HandleAsync(
            [FromRoute] Guid id,
            CashControlDbContext context,
            CancellationToken cancellationToken)
        {
            AccountId accountId = AccountId.Create(id);

            Account? account = await context
                .Accounts
                .SingleOrDefaultAsync(a => a.Id == accountId, cancellationToken);

            if (account is null)
            {
                Result failureResult = Result.Failure(Errors.AccountNotFound(id));
                return TypedResults.NotFound(failureResult);
            }

            account.Delete();
            await context.SaveChangesAsync(cancellationToken);

            return TypedResults.NoContent();
        }
    }
}
