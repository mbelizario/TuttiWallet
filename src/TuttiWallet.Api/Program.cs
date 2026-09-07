using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using TuttiWallet.Api.Autenticacao;
using TuttiWallet.Api.Categorias;
using TuttiWallet.Api.Transacoes;
using TuttiWallet.Api.Usuarios;
using TuttiWallet.Application;
using TuttiWallet.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("Postgres")
    ?? throw new InvalidOperationException("A connection string 'Postgres' não está configurada.");

builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString, builder.Configuration);

var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtChave = jwtSection["Chave"]
    ?? throw new InvalidOperationException("A chave 'Jwt:Chave' não está configurada.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSection["Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtChave)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy("Web", policy => policy
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.Run(async context =>
{
    var excecao = context.Features.Get<IExceptionHandlerFeature>()?.Error;

    var (statusCode, titulo) = excecao is BadHttpRequestException
        ? (StatusCodes.Status400BadRequest, "Não foi possível interpretar os dados enviados na requisição.")
        : (StatusCodes.Status500InternalServerError, "Ocorreu um erro inesperado. Tente novamente mais tarde.");

    if (statusCode == StatusCodes.Status500InternalServerError)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(excecao, "Erro não tratado ao processar a requisição.");
    }

    await Results.Problem(title: titulo, statusCode: statusCode).ExecuteAsync(context);
}));

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseCors("Web");
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapUsuariosEndpoints();
app.MapAutenticacaoEndpoints();
app.MapCategoriasEndpoints();
app.MapTransacoesEndpoints();

app.Run();

public partial class Program;
