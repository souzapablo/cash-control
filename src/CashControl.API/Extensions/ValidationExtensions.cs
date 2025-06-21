using Microsoft.AspNetCore.Mvc;

namespace CashControl.API.Extensions;

public static class ValidationExtensions
{
    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var modelState = context.ModelState;

                var errors = modelState
                    .Where(entry => entry.Value?.Errors.Count > 0)
                    .GroupBy(entry => entry.Key)
                    .ToDictionary(
                        g => g.Key,
                        g => g.SelectMany(e => e.Value!.Errors.Select(err => err.ErrorMessage)).ToArray()
                    );

                var problemDetails = new ValidationProblemDetails(errors)
                {
                    Title = "Validation failed",
                    Status = StatusCodes.Status400BadRequest,
                    Type = "https://httpstatuses.com/400",
                    Detail = "One or more validation errors occurred."
                };

                return new BadRequestObjectResult(problemDetails);
            };
        });
        return services;
    }
}