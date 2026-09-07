using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Dapper;
using FluentAssertions;
using Npgsql;
using TuttiWallet.Contracts.Autenticacao;
using TuttiWallet.Contracts.Categorias;
using TuttiWallet.Contracts.Transacoes;
using TuttiWallet.Contracts.Usuarios;

namespace TuttiWallet.Api.IntegrationTests;

public class CriarTransacaoTests(ApiWebApplicationFactory factory)
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
    public async Task CriarTransacaoSemTokenRetornaUnauthorized()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/transacoes", RequestValido(Guid.NewGuid()));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CriarTransacaoComDadosValidosRetornaCreated()
    {
        var client = await CriarClienteAutenticadoAsync();
        var categoriaId = await CriarCategoriaAsync(client, "Alimentação", tipoId: 2);

        var response = await client.PostAsJsonAsync("/api/transacoes", RequestValido(categoriaId));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var conteudo = await response.Content.ReadFromJsonAsync<TransacaoResponse>();
        conteudo!.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CriarTransacaoSemDescricaoRetornaBadRequestComErroDeCampo()
    {
        var client = await CriarClienteAutenticadoAsync();
        var categoriaId = await CriarCategoriaAsync(client, "Alimentação", tipoId: 2);

        var response = await client.PostAsJsonAsync(
            "/api/transacoes",
            RequestValido(categoriaId, descricao: " "));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CriarTransacaoComValorZeroRetornaBadRequest()
    {
        var client = await CriarClienteAutenticadoAsync();
        var categoriaId = await CriarCategoriaAsync(client, "Alimentação", tipoId: 2);

        var response = await client.PostAsJsonAsync(
            "/api/transacoes",
            RequestValido(categoriaId, valor: 0));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CriarTransacaoComCategoriaInexistenteRetornaBadRequest()
    {
        var client = await CriarClienteAutenticadoAsync();

        var response = await client.PostAsJsonAsync("/api/transacoes", RequestValido(Guid.NewGuid()));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CriarTransacaoComDataOcorrenciaEmFormatoInvalidoRetornaBadRequestSemVazarExcecao()
    {
        var client = await CriarClienteAutenticadoAsync();
        var categoriaId = await CriarCategoriaAsync(client, "Alimentação", tipoId: 2);

        using var content = new StringContent(
            $$"""
            { "categoriaId": "{{categoriaId}}", "valor": 10, "dataOcorrencia": "", "descricao": "Teste" }
            """,
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync("/api/transacoes", content);
        var corpo = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        corpo.Should().NotContain("Exception");
        corpo.Should().NotContain("StackTrace");
        corpo.Should().NotContain("System.Text.Json");
    }

    private static CriarTransacaoRequest RequestValido(Guid categoriaId, decimal valor = 150.75m, string descricao = "Supermercado") => new()
    {
        CategoriaId = categoriaId,
        Valor = valor,
        DataOcorrencia = new DateOnly(2026, 8, 8),
        Descricao = descricao
    };

    private async Task<HttpClient> CriarClienteAutenticadoAsync()
    {
        var client = factory.CreateClient();

        var loginResponse = await client.PostAsJsonAsync(
            "/api/autenticacao/login",
            new LoginRequest { Email = Email, Senha = Senha });
        var login = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login!.AccessToken);

        return client;
    }

    private static async Task<Guid> CriarCategoriaAsync(HttpClient client, string nome, int tipoId)
    {
        var response = await client.PostAsJsonAsync(
            "/api/categorias",
            new CriarCategoriaRequest { Nome = nome, TipoId = tipoId });

        var conteudo = await response.Content.ReadFromJsonAsync<CategoriaResponse>();

        return conteudo!.Id;
    }
}
