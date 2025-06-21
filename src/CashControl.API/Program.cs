using CashControl.API.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddDatabase(builder.Configuration)
    .AddHandlers();

var app = builder.Build();

app.MapControllers();
app.Run();
