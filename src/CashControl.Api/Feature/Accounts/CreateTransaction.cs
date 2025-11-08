using CashControl.Api.Abstractions;
using CashControl.Domain.Accounts;
using CashControl.Domain.Enums;
using CashControl.Domain.Errors;
using CashControl.Domain.Primitives;
using CashControl.Domain.Transactions;
using CashControl.Domain.ValueObjects;
using CashControl.Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CashControl.Api.Feature.Accounts;

public class CreateTransaction
{
    public record Command(
        string Description,
        decimal Amount,
        Currency Currency,
        DateTime Date,
        TransactionType Type
    );

    public record Response(Guid Id);

    public class CreateTransactionEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app) =>
            app.MapPost("/{id:guid}/transactions", HandleAsync)
                .WithName("Transactions: Create")
                .WithSummary("Creates a new transaction")
                .WithDescription("Creates a new transaction with the provided details.");

        private static async Task<
            Results<NotFound<Result>, BadRequest<Result>, Created<Result<Response>>>
        > HandleAsync(
            [FromRoute] Guid id,
            [FromBody] Command command,
            CashControlDbContext context,
            CancellationToken cancellationToken
        )
        {
            Error? validationError = Validate(command);

            if (validationError is not null)
            {
                var failureResult = Result.Failure(validationError);
                return TypedResults.BadRequest(failureResult);
            }

            AccountId accountId = AccountId.Create(id);
            Account? account = await context
                .Accounts.Include(account => account.Transactions)
                .SingleOrDefaultAsync(account => account.Id == accountId, cancellationToken);

            if (account is null)
            {
                var failureResult = Result.Failure(AccountErrors.AccountNotFound(id));
                return TypedResults.NotFound(failureResult);
            }

            if (account.Currency != command.Currency)
            {
                Result failureResult = Result.Failure(TransactionErrors.CurrencyMismatch);
                return TypedResults.BadRequest(failureResult);
            }
            Transaction transaction = Transaction.Create(
                command.Description,
                Money.Create(command.Amount, command.Currency),
                command.Type,
                command.Date
            );

            account.AddTransaction(transaction);
            context.Accounts.Update(account);
            await context.SaveChangesAsync(cancellationToken);

            return TypedResults.Created(
                $"accounts/{id}/transactions/{transaction.Id.Value}",
                Result.Success(new Response(transaction.Id.Value))
            );
        }
    }

    private static Error? Validate(Command command)
    {
        if (string.IsNullOrWhiteSpace(command.Description))
            return Error.ValidationError("The field Description must be informed.");

        if (command.Description.Length > 200)
            return Error.ValidationError(
                "The field Description must be a string with a maximum length of '200'."
            );

        if (!Enum.IsDefined(typeof(Currency), command.Currency))
            return Error.ValidationError("The field Currency must be a valid currency.");

        if (decimal.Compare(command.Amount, 0) <= 0)
            return Error.ValidationError("The field Amount must be greater than zero.");

        if (!Enum.IsDefined(typeof(TransactionType), command.Type))
            return Error.ValidationError("The field Type must be a valid transaction type.");

        return null;
    }
}
