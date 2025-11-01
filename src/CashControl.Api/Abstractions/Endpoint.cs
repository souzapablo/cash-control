using static CashControl.Api.Feature.Accounts.Create;

namespace CashControl.Api.Abstractions;

public static class Endpoint
{
    public static void MapEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("api");

        endpoints.MapGroup("/")
            .WithTags("Health Check")
            .MapGet("/", () => new { message = "OK" });
        
        endpoints.MapGroup("/accounts")
            .WithTags("Accounts")
            .MapEndpoint<CreateAccountEndpoint>();
    }

    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app)
        where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }
}
