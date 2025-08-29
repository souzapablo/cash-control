using CashControl.App.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace CashControl.IntegrationTests.Abstractions;

public class BaseIntegrationTest
    : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly IServiceScope _scope;
    protected readonly AppDbContext DbContext;
    protected readonly HttpClient _client;
    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();

        DbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        _client = factory.CreateClientWithPort();
    }
}