using CashControl.API.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CashControl.API.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("No connection string configured");
        services.AddDbContext<AppDbContext>(cfg => cfg.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());
        return services;
    }
}