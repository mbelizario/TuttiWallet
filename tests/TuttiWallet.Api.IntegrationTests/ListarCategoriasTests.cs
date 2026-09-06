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

public class ListarCategoriasTests(ApiWebApplicationFactory factory)
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
    public async Task ListarCategoriasSemTokenRetornaUnauthorized()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/categorias");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ListarCategoriasSemParametrosUsaValoresPadrao()
    {
        var client = await CriarClienteAutenticadoAsync();
        await CriarCategoriaAsync(client, "Salário", tipoId: 1);
        await CriarCategoriaAsync(client, "Alimentação", tipoId: 2);

        var response = await client.GetAsync("/api/categorias");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var conteudo = await response.Content.ReadFromJsonAsync<ListarCategoriasResponse>();
        conteudo!.Pagina.Should().Be(1);
        conteudo.TamanhoPagina.Should().Be(10);
        conteudo.TotalRegistros.Should().Be(2);
        conteudo.TotalPaginas.Should().Be(1);
        conteudo.Itens.Should().HaveCount(2);
    }

    [Fact]
    public async Task ListarCategoriasRespeitaTamanhoDePaginaEscolhido()
    {
        var client = await CriarClienteAutenticadoAsync();
        for (var i = 1; i <= 5; i++)
            await CriarCategoriaAsync(client, $"Categoria {i}", tipoId: 1);

        var response = await client.GetAsync("/api/categorias?pagina=1&tamanhoPagina=2");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var conteudo = await response.Content.ReadFromJsonAsync<ListarCategoriasResponse>();
        conteudo!.TotalRegistros.Should().Be(5);
        conteudo.TotalPaginas.Should().Be(3);
        conteudo.Itens.Should().HaveCount(2);
    }

    [Fact]
    public async Task ListarCategoriasNaSegundaPaginaRetornaItensDiferentesDaPrimeira()
    {
        var client = await CriarClienteAutenticadoAsync();
        for (var i = 1; i <= 5; i++)
            await CriarCategoriaAsync(client, $"Categoria {i}", tipoId: 1);

        var respostaPagina1 = await client.GetAsync("/api/categorias?pagina=1&tamanhoPagina=2");
        var respostaPagina2 = await client.GetAsync("/api/categorias?pagina=2&tamanhoPagina=2");

        var pagina1 = await respostaPagina1.Content.ReadFromJsonAsync<ListarCategoriasResponse>();
        var pagina2 = await respostaPagina2.Content.ReadFromJsonAsync<ListarCategoriasResponse>();

        pagina1!.Itens.Select(item => item.Id).Should().NotIntersectWith(pagina2!.Itens.Select(item => item.Id));
    }

    [Fact]
    public async Task ListarCategoriasIndicaCategoriaPaiDasSubcategorias()
    {
        var client = await CriarClienteAutenticadoAsync();
        var categoriaPrincipalId = await CriarCategoriaAsync(client, "Alimentação", tipoId: 2);
        await CriarCategoriaAsync(client, "Restaurante", tipoId: 2, categoriaPaiId: categoriaPrincipalId);

        var response = await client.GetAsync("/api/categorias");

        var conteudo = await response.Content.ReadFromJsonAsync<ListarCategoriasResponse>();
        conteudo!.Itens.Single(item => item.Nome == "Alimentação").CategoriaPaiId.Should().BeNull();
        conteudo.Itens.Single(item => item.Nome == "Restaurante").CategoriaPaiId.Should().Be(categoriaPrincipalId);
    }

    [Fact]
    public async Task ListarCategoriasComPaginaInvalidaRetornaBadRequest()
    {
        var client = await CriarClienteAutenticadoAsync();

        var response = await client.GetAsync("/api/categorias?pagina=0");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ListarCategoriasComTamanhoPaginaAcimaDoLimiteRetornaBadRequest()
    {
        var client = await CriarClienteAutenticadoAsync();

        var response = await client.GetAsync("/api/categorias?tamanhoPagina=101");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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
