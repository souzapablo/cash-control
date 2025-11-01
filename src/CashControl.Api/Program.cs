using CashControl.Api.Abstractions;
using CashControl.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Cash Control API",
        Version = "v1",
        Description = "A simple personal finance app API"
    });
    
    c.CustomSchemaIds(type => type.FullName?.Replace("+", ".") ?? type.Name);
});

var configuration = builder.Configuration;  
builder.Services.AddInfrastructureModules(configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapEndpoints();

app.Run();
