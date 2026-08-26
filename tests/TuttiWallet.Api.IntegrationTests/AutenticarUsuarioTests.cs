using System.Net;
using System.Net.Http.Json;
using Dapper;
using FluentAssertions;
using Npgsql;
using TuttiWallet.Contracts.Autenticacao;
using TuttiWallet.Contracts.Usuarios;

namespace TuttiWallet.Api.IntegrationTests;

public class AutenticarUsuarioTests(ApiWebApplicationFactory factory)
    : IClassFixture<ApiWebApplicationFactory>, IAsyncLifetime
{
    private const string Email = "maria@exemplo.com";
    private const string Senha = "abcX24";

    public async Task InitializeAsync()
    {
        await using var conexao = new NpgsqlConnection(factory.ConnectionString);
        await conexao.OpenAsync();
        await conexao.ExecuteAsync("TRUNCATE TABLE Usuarios CASCADE;");

        var client = factory.CreateClient();
        await client.PostAsJsonAsync("/api/usuarios/primeiro-acesso", new CadastrarPrimeiroUsuarioRequest
        {
            Nome = "Maria",
            Sobrenome = "Silva",
            Email = Email,
            Celular = "(11) 98765-4321",
            Senha = Senha,
            ConfirmacaoSenha = Senha
        });
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task AutenticarComCredenciaisValidasRetornaOkComTokens()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/autenticacao/login", new LoginRequest { Email = Email, Senha = Senha });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var conteudo = await response.Content.ReadFromJsonAsync<LoginResponse>();
        conteudo!.AccessToken.Should().NotBeNullOrWhiteSpace();
        conteudo.RefreshToken.Should().NotBeNullOrWhiteSpace();
        conteudo.ExpiraEm.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task AutenticarComSenhaInvalidaRetornaUnauthorized()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/autenticacao/login", new LoginRequest { Email = Email, Senha = "senha-errada" });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AutenticarComEmailInexistenteRetornaUnauthorized()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/autenticacao/login",
            new LoginRequest { Email = "inexistente@exemplo.com", Senha = Senha });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
