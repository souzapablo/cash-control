using CashControl.App.Abstractions;
using CashControl.App.Features.Users.Commands;

namespace CashControl.App.Features;

public static class Endpoints
{
    public static void MapEndpoints(this WebApplication app)
    {
        var endpoints = app
            .MapGroup("api");

        endpoints.MapGroup("v1/users")
            .WithTags("Users")
            .MapEndpoint<RegisterUserEndpoint>();
    }
    
    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app)
        where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }
}
