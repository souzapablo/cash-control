using CashControl.Api.Abstractions;
using CashControl.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;  
builder.Services.AddInfrastructureModules(configuration);

var app = builder.Build();

app.MapEndpoints();

app.Run();
