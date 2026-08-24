using System.Net;
using System.Net.Http.Json;
using Dapper;
using FluentAssertions;
using Npgsql;
using TuttiWallet.Contracts.Usuarios;

namespace TuttiWallet.Api.IntegrationTests;

public class CadastrarPrimeiroUsuarioTests(ApiWebApplicationFactory factory)
    : IClassFixture<ApiWebApplicationFactory>, IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await using var conexao = new NpgsqlConnection(factory.ConnectionString);
        await conexao.OpenAsync();
        await conexao.ExecuteAsync("TRUNCATE TABLE Usuarios;");
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task CadastrarPrimeiroUsuarioComDadosValidosRetornaCreated()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/usuarios/primeiro-acesso", RequestValido(email: "maria@exemplo.com"));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CadastrarPrimeiroUsuarioComSenhaSemNumeroRetornaBadRequestComErroDeCampo()
    {
        var client = factory.CreateClient();
        var request = RequestValido(email: "outro@exemplo.com", senha: "abcdef");

        var response = await client.PostAsJsonAsync("/api/usuarios/primeiro-acesso", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CadastrarSegundoUsuarioRetornaConflict()
    {
        var client = factory.CreateClient();
        await client.PostAsJsonAsync("/api/usuarios/primeiro-acesso", RequestValido(email: "primeiro@exemplo.com"));

        var response = await client.PostAsJsonAsync(
            "/api/usuarios/primeiro-acesso",
            RequestValido(email: "segundo@exemplo.com"));

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    private static CadastrarPrimeiroUsuarioRequest RequestValido(string email, string senha = "abcX24") => new()
    {
        Nome = "Maria",
        Sobrenome = "Silva",
        Email = email,
        Celular = "(11) 98765-4321",
        Senha = senha,
        ConfirmacaoSenha = senha
    };
}
