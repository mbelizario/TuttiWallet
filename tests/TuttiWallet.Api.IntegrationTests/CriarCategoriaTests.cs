using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Dapper;
using FluentAssertions;
using Npgsql;
using TuttiWallet.Contracts.Autenticacao;
using TuttiWallet.Contracts.Categorias;
using TuttiWallet.Contracts.Usuarios;

namespace TuttiWallet.Api.IntegrationTests;

public class CriarCategoriaTests(ApiWebApplicationFactory factory)
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
    public async Task CriarCategoriaSemTokenRetornaUnauthorized()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/categorias",
            new CriarCategoriaRequest { Nome = "Salário", TipoId = 1 });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CriarCategoriaPrincipalComDadosValidosRetornaCreated()
    {
        var client = await CriarClienteAutenticadoAsync();

        var response = await client.PostAsJsonAsync(
            "/api/categorias",
            new CriarCategoriaRequest { Nome = "Salário", TipoId = 1 });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var conteudo = await response.Content.ReadFromJsonAsync<CategoriaResponse>();
        conteudo!.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CriarSubcategoriaComMesmoTipoDoPaiRetornaCreated()
    {
        var client = await CriarClienteAutenticadoAsync();
        var categoriaPaiId = await CriarCategoriaAsync(client, "Alimentação", tipoId: 2);

        var response = await client.PostAsJsonAsync(
            "/api/categorias",
            new CriarCategoriaRequest { Nome = "Restaurante", TipoId = 2, CategoriaPaiId = categoriaPaiId });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CriarCategoriaSemNomeRetornaBadRequestComErroDeCampo()
    {
        var client = await CriarClienteAutenticadoAsync();

        var response = await client.PostAsJsonAsync(
            "/api/categorias",
            new CriarCategoriaRequest { Nome = " ", TipoId = 1 });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CriarCategoriaComTipoIdInvalidoRetornaBadRequest()
    {
        var client = await CriarClienteAutenticadoAsync();

        var response = await client.PostAsJsonAsync(
            "/api/categorias",
            new CriarCategoriaRequest { Nome = "Salário", TipoId = 99 });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CriarCategoriaComPaiInexistenteRetornaBadRequest()
    {
        var client = await CriarClienteAutenticadoAsync();

        var response = await client.PostAsJsonAsync(
            "/api/categorias",
            new CriarCategoriaRequest { Nome = "Restaurante", TipoId = 2, CategoriaPaiId = Guid.NewGuid() });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CriarCategoriaComPaiQueJaEhSubcategoriaRetornaBadRequest()
    {
        var client = await CriarClienteAutenticadoAsync();
        var categoriaPrincipalId = await CriarCategoriaAsync(client, "Alimentação", tipoId: 2);
        var subcategoriaId = await CriarCategoriaAsync(client, "Restaurante", tipoId: 2, categoriaPaiId: categoriaPrincipalId);

        var response = await client.PostAsJsonAsync(
            "/api/categorias",
            new CriarCategoriaRequest { Nome = "Lanche", TipoId = 2, CategoriaPaiId = subcategoriaId });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CriarSubcategoriaComTipoDiferenteDoPaiRetornaBadRequest()
    {
        var client = await CriarClienteAutenticadoAsync();
        var categoriaPaiId = await CriarCategoriaAsync(client, "Alimentação", tipoId: 2);

        var response = await client.PostAsJsonAsync(
            "/api/categorias",
            new CriarCategoriaRequest { Nome = "Salário", TipoId = 1, CategoriaPaiId = categoriaPaiId });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CriarCategoriaComNomeJaExistenteNormalizadoRetornaBadRequest()
    {
        var client = await CriarClienteAutenticadoAsync();
        await CriarCategoriaAsync(client, "Fundos Imobiliários", tipoId: 1);

        var response = await client.PostAsJsonAsync(
            "/api/categorias",
            new CriarCategoriaRequest { Nome = "Fundos Imobiliarios", TipoId = 1 });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CriarSubcategoriaComMesmoNomeEmPaisDiferentesRetornaCreated()
    {
        var client = await CriarClienteAutenticadoAsync();
        var alimentacaoId = await CriarCategoriaAsync(client, "Alimentação", tipoId: 2);
        var lazerId = await CriarCategoriaAsync(client, "Lazer", tipoId: 2);
        await CriarCategoriaAsync(client, "Outros", tipoId: 2, categoriaPaiId: alimentacaoId);

        var response = await client.PostAsJsonAsync(
            "/api/categorias",
            new CriarCategoriaRequest { Nome = "Outros", TipoId = 2, CategoriaPaiId = lazerId });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

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

    private static async Task<Guid> CriarCategoriaAsync(HttpClient client, string nome, int tipoId, Guid? categoriaPaiId = null)
    {
        var response = await client.PostAsJsonAsync(
            "/api/categorias",
            new CriarCategoriaRequest { Nome = nome, TipoId = tipoId, CategoriaPaiId = categoriaPaiId });

        var conteudo = await response.Content.ReadFromJsonAsync<CategoriaResponse>();

        return conteudo!.Id;
    }
}
