using CashControl.App.Features;
using CashControl.App.Features.Users;
using CashControl.App.Features.Users.Commands;
using CashControl.App.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddScoped<IRegisterUserHandler, RegisterUserHandler>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x =>
{
    x.SwaggerDoc("v1", new()
    {
        Title = "Cash Control API",
        Version = "v1",
        Description = "A simple API for managing personal finances",
        Contact = new OpenApiContact
        {
            Name = "Pablo Souza",
            Email = "contato@souzapablo.dev.br",
            Url = new Uri("https://github.com/souzapablo/")
        }
    });
});
var connectionString = configuration.GetConnectionString("Postgres") ?? throw new InvalidOperationException();
builder.Services.AddDbContext<AppDbContext>(cfg => cfg.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Hello World!");

app.MapEndpoints();

app.Run();
