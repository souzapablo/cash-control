using CashControl.Api.Abstractions;
using CashControl.Api.Features.Users.Commands;

namespace CashControl.Api.Features;

public static class Endpoints
{
    public static void MapEndpoints(this WebApplication app)
    {
        var endpoints = app
            .MapGroup("");

        endpoints.MapGroup("v1/users")
            .WithTags("Users")
            .MapEndpoint<RegisterUserEnpdoint>();
    }
    
    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app)
        where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }
}
