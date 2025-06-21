using CashControl.API.Abstractions;
using CashControl.API.Abstractions.Messaging;
using CashControl.API.Data.Persistence;
using CashControl.API.Data.Persistence.Repositories;
using CashControl.API.Domain.Repositories;
using CashControl.API.Features.Users.Commands.Create;
using Microsoft.EntityFrameworkCore;

namespace CashControl.API.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("No connection string configured");
        services.AddDbContext<AppDbContext>(cfg => cfg.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        AddRepositories(services);
        
        return services;
    }

    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateUserCommand, long>, CreateUserCommandHandler>();
        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        return services;
    }
}