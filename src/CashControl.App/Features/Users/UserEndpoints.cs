using CashControl.App.Abstractions;

namespace CashControl.App.Features.Users;

public static class UserEndpoints
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapGroup("api/v1/users")
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
