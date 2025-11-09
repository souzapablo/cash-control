using CashControl.Api.Abstractions;
using CashControl.Domain.Categories;
using CashControl.Domain.Primitives;
using CashControl.Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CashControl.Api.Feature.Categories;

public class Create
{
    public record Command(string Name);

    public record Response(Guid Id);

    public class CreateCategoryEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app) =>
            app.MapPost("", HandleAsnyc)
                .WithName("Categories: Create")
                .WithSummary("Creates a new category")
                .WithDescription("Creates a new category with the given name.");

        private static async Task<
            Results<BadRequest<Result>, Created<Result<Response>>>
        > HandleAsnyc(
            Command command,
            CashControlDbContext dbContext,
            CancellationToken cancellationToken
        )
        {
            Error? error = Validate(command);

            if (error is not null)
            {
                Result failureResult = Result.Failure(error);
                return TypedResults.BadRequest(failureResult);
            }

            var category = Category.Create(command.Name);
            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync(cancellationToken);
            var result = Result.Success(new Response(category.Id));
            return TypedResults.Created($"/categories/{category.Id.Value}", result);
        }

        private static Error? Validate(Command command)
        {
            if (string.IsNullOrWhiteSpace(command.Name))
                return Error.ValidationError("Category name cannot be empty.");

            if (command.Name.Length > 200)
                return Error.ValidationError("Category name cannot exceed 200 characters.");

            return null;
        }
    }
}
