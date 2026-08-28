using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using TuttiWallet.Migrator;

namespace TuttiWallet.Api.IntegrationTests;

public sealed class ApiWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:16-alpine")
        .Build();

    public string ConnectionString => _postgres.GetConnectionString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:Postgres", ConnectionString);
        builder.UseSetting("Jwt:Chave", "chave-secreta-para-testes-de-integracao-0123456789");
    }

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        ConnectionString.AplicarScripts();
    }

    async Task IAsyncLifetime.DisposeAsync() => await _postgres.DisposeAsync();
}
