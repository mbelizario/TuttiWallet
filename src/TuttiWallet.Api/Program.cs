using Scalar.AspNetCore;
using TuttiWallet.Api.Usuarios;
using TuttiWallet.Application;
using TuttiWallet.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("Postgres")
    ?? throw new InvalidOperationException("A connection string 'Postgres' não está configurada.");

builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString);

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy("Web", policy => policy
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseCors("Web");

app.MapHealthChecks("/health");
app.MapUsuariosEndpoints();

app.Run();

public partial class Program;
