using Scalar.AspNetCore;
using Serilog;
using TuttiWallet.Api.Autenticacao;
using TuttiWallet.Api.Categorias;
using TuttiWallet.Api.Cors;
using TuttiWallet.Api.Logging;
using TuttiWallet.Api.TratamentoDeExcecoes;
using TuttiWallet.Api.Transacoes;
using TuttiWallet.Api.Usuarios;
using TuttiWallet.Application;
using TuttiWallet.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.UseSerilogConfigurado();

builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("Postgres")
    ?? throw new InvalidOperationException("A connection string 'Postgres' não está configurada.");

builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString, builder.Configuration);
builder.Services.AddAutenticacaoJwt(builder.Configuration);
builder.Services.AddCorsConfigurado(builder.Configuration);
builder.Services.AddTratamentoDeExcecoes();

var app = builder.Build();

app.UseTratamentoDeExcecoes();
app.UseSerilogRequestLogging();

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
