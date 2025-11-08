using CashControl.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CashControl.IntegrationTests.Infrastructure;

public class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly IServiceScope _scope;
    protected readonly CashControlDbContext Context;
    protected readonly HttpClient Client;

    public BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        Context = _scope.ServiceProvider.GetRequiredService<CashControlDbContext>();
        Client = factory.CreateClient();
    }
}
